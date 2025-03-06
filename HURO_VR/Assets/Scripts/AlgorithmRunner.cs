using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using TMPro;
using Microsoft.Scripting.Hosting;
using System.Runtime.CompilerServices;
using IronPython.Hosting;
using IronPython.Runtime;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using static Community.CsharpSqlite.Sqlite3;
using Meta.XR.MRUtilityKit;



[RequireComponent(typeof(SceneDataManager))]
[RequireComponent(typeof(GoogleCloudServer))]

public class AlgorithmRunner : MonoBehaviour {


    private ScriptEngine engine;
    private dynamic algorithm;
    bool initAlgorithm = false;
    SceneDataManager sceneData;
    bool algorithmRunning = false;
    [SerializeField] bool displayGizmos;
    AudioLibrary audioLibrary;
    [SerializeField] TextMeshProUGUI debugLogs;
    [SerializeField] bool runOnServer;
    bool hitServerAgain = true;
    GoogleCloudServer remoteScriptExecutor;
    StreamingAssetsManager fileTransfer;
    SessionController sessionController;
    [SerializeField] List<FindSpawnPositions> spawners;

    private void Awake()
    {
        // Ensure server is enabled at Runtime on Quest.
        #if UNITY_ANDROID
            runOnServer = true;
        #endif

        try
        {
            if (!sceneData) sceneData = GetComponent<SceneDataManager>();
            audioLibrary = FindAnyObjectByType<AudioLibrary>();
            remoteScriptExecutor = GetComponent<GoogleCloudServer>();
            fileTransfer = FindAnyObjectByType<StreamingAssetsManager>();
            sessionController = FindAnyObjectByType<SessionController>();
        }
        catch (Exception e)
        {
            DebugLogs(e.ToString());
        }
        
    }

    public bool IsRunning()
    {
        return algorithmRunning;
    }

    void SetImportPaths(ScriptEngine engine)
    {
        ICollection<string> searchPaths = engine.GetSearchPaths();

        //Path to the folder of filename
        searchPaths.Add(Application.streamingAssetsPath + @"/Python/");

        //Path to the Python standard library
        searchPaths.Add(Application.dataPath + @"/Plugins/IronPy/Lib/");

        engine.SetSearchPaths(searchPaths);
    }

    public void InitAlgorithm()
    {
        SimulationDataCollector.InitializeSimulation();
        if (runOnServer) { 
            remoteScriptExecutor.OpenSSHConnection(); 
        }
        else
        {
            engine = Python.CreateEngine();
            SetImportPaths(engine);
            algorithm = engine.ExecuteFile(Application.streamingAssetsPath + @"/Python/main.py");
        }

        if (!sceneData) sceneData = GetComponent<SceneDataManager>();
        sceneData.InitSceneData();
        
        initAlgorithm = true;
    }

    public void StartAlgorithm()
    {
        if (audioLibrary && !algorithmRunning) audioLibrary.PlayAudio(AudioLibrary.AudioType.StartSimulation);
        algorithmRunning = true;
        Debug.Log("HURO: Starting simulation");
    }

    public void ToggleAlgorithm()
    {
        bool start = !algorithmRunning;
        Debug.Log("Toggle with " + start);
        if (start) StartAlgorithm();
        else PauseAlgorithm();
    }

    void PauseAlgorithm()
    {
        algorithmRunning = false;
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject robot in robots)
        {
            Rigidbody rigidbody = robot.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }
        timer = 0f;
    }

    void RunAlgorithmLocally(string input)
    {
        string output =  algorithm.main(input);
        OnScriptComplete(output);
    }

    void OnScriptComplete(string output)
    {
        if (output.Length == 0)
        {
            Debug.LogWarning("SSH Command did not output any data.");
        } else
        {
            var newVelocities = JsonConvert.DeserializeObject<float[][]>(output);
            SetNewVelocities(newVelocities);
            if (runOnServer) hitServerAgain = true;
        }
    }


    private void RunAlgorithmOnServer(string param)
    {
        remoteScriptExecutor.OnScriptExecutionComplete += OnScriptComplete;
        //remoteScriptExecutor.EstablishSshConnection();
        remoteScriptExecutor.ExecuteCommand(param);
        
    }

    void SetNewVelocities(float[][] newVelocities)
    {
        int stopSim = 0;
        GameObject[] robots_go = GameObject.FindGameObjectsWithTag("Robot");
        foreach (var go in robots_go)
        {
            int i = 0;
            foreach (var robot in sceneData.data.robots)
            {
                if (newVelocities[i][0] + newVelocities[i][1] == 0)
                {
                    stopSim++;
                    continue;
                }
                if (go.name == robot.name)
                {
                    go.GetComponent<RobotController>().SetVelocity(newVelocities[i][0], newVelocities[i][1]);
                }
                i++;
            }
        }
        if (stopSim == robots_go.Length)
        {
            Debug.LogWarning("Robots stopped");
        }
    }

    void EndSimulation()
    {
        SimulationDataCollector.EndSimulation();
        PauseAlgorithm();
        sessionController?.UploadSimulationRunData(SimulationDataCollector.simulationRun);
        audioLibrary.PlayAudio(AudioLibrary.AudioType.EndSimulation);
        RandomizeObjectLocations();
    }

    private float timer = 0f;
    private float totalTime = 0f;

    [Tooltip("Runs the algorithm every X seconds.")]
    [SerializeField] float interval = 0.25f; // Run every x seconds
    [SerializeField] float simulationTimeout = 30f;

    bool ShouldTerminate()
    {
        bool timeout = totalTime > simulationTimeout;
        bool reachedGoals = SimulationDataCollector.CheckAllRobotsReachedGoal();
        bool deadlock = true;
        RobotController[] robots = FindObjectsByType<RobotController>(FindObjectsSortMode.InstanceID);
        foreach (var robot in robots)
        {
            if (robot.stuck) deadlock = false;
        }
        return reachedGoals;
        return (timeout || reachedGoals || deadlock);
        
    }

    public void RandomizeObjectLocations()
    {
        foreach (var spawner in spawners)
        {
            for (int i = 0; i < spawner.transform.childCount; i++)
            {
                Destroy(spawner.transform.GetChild(i).gameObject);
            }
            spawner.StartSpawn();
        }
    }

    void IncrementTime()
    {
        timer += Time.deltaTime; // Accumulate time
        totalTime += Time.deltaTime;
    }

    // Update is called once per frame
    void Update () {
        DetectKeyBoardActivation();
        if (!algorithmRunning || !fileTransfer.IsCopyingComplete()) return;
        IncrementTime();
        if (ShouldTerminate()) EndSimulation();

        if (timer >= interval)
        {
            timer = 0f; // Reset timer
            try
            {
                if (!initAlgorithm) InitAlgorithm();
                if ((runOnServer && hitServerAgain) || !runOnServer) {
                    sceneData.UpdateSceneData();
                    SimulationDataCollector.UpdateRobotData();
                    string input = JsonConvert.SerializeObject(sceneData.data);
                    if (runOnServer && hitServerAgain) {
                        hitServerAgain = false;
                        Debug.Log("Calling Run Algorithim On Server");
                        RunAlgorithmOnServer(input); 
                    }
                    else if (!runOnServer) RunAlgorithmLocally(input);
                }
            } catch (Exception e)
            {
                DebugLogs(e.ToString());
                PauseAlgorithm();
                DebugLogs("Stopping Simulation");
            }
        }
    }

    private void DebugLogs(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        debugLogs.text += "\n\n" + message;
        Debug.Log($"HURO: {file} @line:{line}: " + message);
    }


    private void OnDrawGizmos()
    {
        if (displayGizmos && sceneData)
        {
            sceneData.DrawGizmos();
        }
    }

    private void DetectKeyBoardActivation()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleAlgorithm();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            InitAlgorithm();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomizeObjectLocations();
        }
    }

    private void OnDestroy()
    {
        remoteScriptExecutor?.CloseSSHConnection();
    }
}
