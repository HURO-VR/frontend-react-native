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

    private void Awake()
    {
        #if !UNITY_EDITOR
        runOnServer = true;
        #endif
        try
        {
            if (!sceneData) sceneData = GetComponent<SceneDataManager>();
            audioLibrary = FindAnyObjectByType<AudioLibrary>();
            remoteScriptExecutor = GetComponent<GoogleCloudServer>();
        }
        catch (Exception e)
        {
            DebugLogs(e.ToString());
        }
        
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
        
        if (runOnServer) remoteScriptExecutor.OpenSSHConnection();
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
        if (audioLibrary && start) audioLibrary.PlayAudio(AudioLibrary.AudioType.StartSimulation);
        algorithmRunning = start;
        if (!start) PauseAlgorithm();
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
    }

    void RunAlgorithm(string input)
    {
        string output =  algorithm.main(input);
        OnScriptComplete(output);
    }

    void OnScriptComplete(string output)
    {
        var newVelocities = JsonConvert.DeserializeObject<float[][]>(output);
        SetNewVelocities(newVelocities);
        if (runOnServer) hitServerAgain = true;
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

    private float timer = 0f;
    private float interval = 0.25f; // Run every x seconds
    // Update is called once per frame
    void FixedUpdate () {
        if (!algorithmRunning) return;

        timer += Time.fixedDeltaTime; // Accumulate time
        if (timer >= interval)
        {
            timer = 0f; // Reset timer
            try
            {
                if (!initAlgorithm) InitAlgorithm();
                if ((runOnServer && hitServerAgain) || !runOnServer) {
                    sceneData.UpdateSceneData();
                    string input = JsonConvert.SerializeObject(sceneData.data);
                    if (runOnServer && hitServerAgain) {
                        hitServerAgain = false;
                        Debug.Log("Calling Run Algorithim On Server");
                        RunAlgorithmOnServer(input); 
                    }
                    else if (!runOnServer) RunAlgorithm(input);
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || OVRInput.GetDown(OVRInput.RawButton.A))
        {
            StartAlgorithm();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            InitAlgorithm();
        }
    }

    private void OnDestroy()
    {
        remoteScriptExecutor?.CloseSSHConnection();
    }
}
