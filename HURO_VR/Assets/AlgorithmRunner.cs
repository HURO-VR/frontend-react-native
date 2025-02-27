using System.Collections.Generic;
using IronPython.Hosting;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Scripting.Hosting;
using Unity.VisualScripting;



[RequireComponent(typeof(SceneDataManager))]
public class AlgorithmRunner : MonoBehaviour {


    private readonly ScriptEngine engine = Python.CreateEngine();
    private dynamic algorithm;
    bool initAlgorithm = false;
    SceneDataManager sceneData;
    bool algorithmRunning = false;
    [SerializeField] bool displayGizmos;

    private void Awake()
    {

        if (!sceneData) sceneData = GetComponent<SceneDataManager>();
    }

    void SetImportPaths(ScriptEngine engine)
    {
        ICollection<string> searchPaths = engine.GetSearchPaths();

        //Path to the folder of filename
        searchPaths.Add(Application.dataPath);

        //Path to the Python standard library
        searchPaths.Add(Application.dataPath + @"\Plugins\IronPy\Lib\");

        engine.SetSearchPaths(searchPaths);
    }

    public void InitAlgorithm()
    {
        Debug.Log("Initializing Main Function at " + "main.py");
        SetImportPaths(engine);
        algorithm = engine.ExecuteFile(Application.dataPath + @"\main.py");
        if (!sceneData) sceneData = GetComponent<SceneDataManager>();
        sceneData.InitSceneData();
        initAlgorithm = true;
    }

    public void StartAlgorithm()
    {
        algorithmRunning = true;
    }

    void RunRVOAlgorithm()
    {
        sceneData.UpdateSceneData();
        string input = JsonConvert.SerializeObject(sceneData.data);
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
    int step = 0;
    // Update is called once per frame
    void FixedUpdate () {
        if (!algorithmRunning) return;

        timer += Time.fixedDeltaTime; // Accumulate time
        if (timer >= interval)
        {
            timer = 0f; // Reset timer
            if (!initAlgorithm) InitAlgorithm();
            RunRVOAlgorithm();
        }
    }


    private void OnDrawGizmos()
    {
        if (displayGizmos && sceneData && sceneData.data != null)
        {
            sceneData.DrawGizmos();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartAlgorithm();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            InitAlgorithm();
        }
    }
}
