using Unity.MLAgents;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;
using System.Threading;
using Unity.MLAgents.Policies;
using UnityEditor;
using Unity.Barracuda;


public class ModelTester : MonoBehaviour
{
    private AgentWrapper agent;
    public GameObject MLenvironment;
    public NNModel model;
    public int RunsPerModel = 100;
    string modelName;
    Storage db;
    byte[] modelData;

    public void Awake()
    {
        db = FindAnyObjectByType<Storage>();
        if (db == null )
        {
            db = gameObject.AddComponent<Storage>();
        }
    }

    private void Update()
    {
        if (agent && agent.numRuns >= RunsPerModel)
        {
            agent.PrintStats();
            MLenvironment.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Getting sim bundles");
            var allSimBundles = db.GetAllSimulationBundles().Result;
            Debug.Log("Returned " + allSimBundles.Length + " bundles");
            if (allSimBundles.Length > 0) LoadSimulationFromMetaData(allSimBundles[0]);
            Debug.Log("Model tester running");
        }
    }

    public void LoadSimulationFromMetaData(Database_Models.SimulationMetaData simMetaData)
    {
        db.DownloadFile(simMetaData.algorithmName, FileType.Algorithm, simMetaData.ID, (data) =>
        {
            Debug.Log("Downloaded model data");
            modelData = data;
        });
        NNModel model = OnnxDataToNNModel(modelData);
        Debug.Log("Loaded model with data");
        SetMLEnvironment(simMetaData.environmentName);
        SetModel(model, simMetaData.algorithmName);
    }


    void InitalizeEnvironment()
    {
        GameObject env = Instantiate(MLenvironment);
        env.transform.position = Vector3.zero;
        agent = env.GetComponentInChildren<AgentWrapper>();
    }

    void InitializeAgent()
    {
        agent.SetModel(modelName, model);
        print($"Testing model: {modelName}");
        agent.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
        agent.maxRuns = RunsPerModel;
        agent.SetTesting(true);
    }


    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }


    void SetMLEnvironment(string environmentName)
    {
        environmentName = "Empty_Room";
        MLenvironment = Resources.Load<GameObject>($"Training_Environments/{environmentName}"); ;
        InitalizeEnvironment();
    }

    void SetModel(NNModel _model, string name)
    {
        model = _model;
        modelName = name;
        InitializeAgent();
    }

    NNModel OnnxDataToNNModel(byte[] data)
    {
        NNModel nnModel = ScriptableObject.CreateInstance<NNModel>();
        nnModel.modelData.Value = data;
        return nnModel;
    }

    [MenuItem("Tools/Stop Play Mode")]
    public static void StopPlay()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
    }
}

public class AgentWrapper : Agent
{
    float numFails;
    float numSuccesses;
    public bool _testing { get; private set; }
    public float numRuns { get; private set; }
    public int maxRuns { private get; set; }
    public void Start()
    {
        numFails = 0;
        numSuccesses = 0;
        numRuns = 0;
    }
    public void LogFail()
    {
        numFails++;
    }

    public void LogSuccess()
    {
        numSuccesses++;
    }

    public void LogRun ()
    {
        numRuns++;
    }

    public void SetTesting(bool test)
    {
        if (numRuns != 0)
        {
            Debug.LogWarning("Cannot Set Testing in the middle of a run. Ignoring Command.");
            return;
        }
        Debug.Log("Model set to testing");
        _testing = test;
    }

    public SimulationStatistics GetStats()
    {
        SimulationStatistics stats = new SimulationStatistics();
        stats.percentSuccess = (numSuccesses / numRuns) * 100;
        stats.percentFailure = (numFails / numRuns) * 100;
        stats.percentDeadlock = (Mathf.Abs(numSuccesses + numFails - numRuns) / numRuns) * 100;
        return stats;
    }

    public void PrintStats()
    {
        SimulationStatistics stats = GetStats();
        print("Percentage success: " + stats.percentSuccess);
        print("Percentage failed: " + stats.percentFailure);
        print("Percentage deadlock: " + stats.percentDeadlock);
    }

    public void ResetStats()
    {
        numFails = 0;
        numSuccesses = 0;
        numRuns = 0;
    }

    public struct SimulationStatistics
    {
        public float percentSuccess;
        public float percentFailure;
        public float percentDeadlock;
    }
}
