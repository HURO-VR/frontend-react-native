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



[RequireComponent(typeof(SceneDataManager))]
public class AlgorithmRunner : MonoBehaviour {


    private ScriptEngine engine;
    private dynamic algorithm;
    bool initAlgorithm = false;
    SceneDataManager sceneData;
    bool algorithmRunning = false;
    [SerializeField] bool displayGizmos;
    AudioLibrary audioLibrary;
    [SerializeField] TextMeshProUGUI debugLogs;

    private void Awake()
    {
        try
        {
            if (!sceneData) sceneData = GetComponent<SceneDataManager>();
            audioLibrary = FindAnyObjectByType<AudioLibrary>();
            engine = Python.CreateEngine();
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
        searchPaths.Add(Application.streamingAssetsPath);

        //Path to the Python standard library
        searchPaths.Add(Application.streamingAssetsPath + @"/Python/Lib/");

        searchPaths.Add(Application.streamingAssetsPath + @"/Python/");

        engine.SetSearchPaths(searchPaths);
    }

    public void InitAlgorithm()
    {
        Debug.Log("Initializing Main Function at " + "main.py");
        SetImportPaths(engine);
        algorithm = engine.ExecuteFile(Application.streamingAssetsPath + @"/Python/main.py");
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
    }

    void RunRVOAlgorithm()
    {
        sceneData.UpdateSceneData();
        string input = JsonConvert.SerializeObject(sceneData.data);
        // Debug.Log(input);
        string output = algorithm.main(input);
        var newVelocities = JsonConvert.DeserializeObject<float[][]>(output);
        SetNewVelocities(newVelocities);
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
                RunRVOAlgorithm();
            } catch (Exception e)
            {
                DebugLogs(e.ToString());
                algorithmRunning = false;
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
}
