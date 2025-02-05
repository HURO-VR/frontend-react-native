using Unity.MLAgents;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build;
using System.Threading;
using Unity.MLAgents.Policies;
using UnityEditor;
using Unity.Barracuda;
using UnityEditor.Experimental.GraphView;
using Unity.Barracuda.ONNX;
using System.IO;


public class ModelTester : MonoBehaviour
{
    private AgentWrapper agent;
    public GameObject MLenvironment;
    private NNModel model;
    public int RunsPerModel = 100;
    string algorithmName;
    Storage db;
    byte[] modelData;
    public NNModel model_template;
    Database_Models.SimulationMetaData[] simMetaDatas;
    bool downloadedModelData = false;
    bool simulationRunning = false;


    public void Awake()
    {
        db = FindAnyObjectByType<Storage>();
        if (db == null )
        {
            db = gameObject.AddComponent<Storage>();
        }
    }

    private void Start()
    {
        Debug.Log("Running ModelTester.");

    }

    private void Update()
    {
        if (agent && agent.numRuns >= RunsPerModel)
        {
            agent.PrintStats();
            MLenvironment.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Pressed esc");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
           RunSimulation();
        }

        if (simMetaDatas != null && simMetaDatas.Length > 0 && downloadedModelData && !simulationRunning)
        {
            simulationRunning = true;
            Debug.Log("Loading sim bundle " + simMetaDatas[0].name);
            LoadSimulationFromMetaData(simMetaDatas[0]);
        }

    }

    bool metaDataRecieved = false;
    async void RunSimulation()
    {
        Debug.Log("Getting sim bundles");
        simMetaDatas = await db.GetAllSimulationBundles();
        Debug.Log("Returned " + simMetaDatas.Length + " bundles");
        metaDataRecieved = true;
        db.DownloadFile(simMetaDatas[0].algorithmName, FileType.Algorithm, simMetaDatas[0].ID, (data) =>
        {
            modelData = data;
            downloadedModelData = true;
            Debug.Log("Downloaded model data");
        });
    }

    public void LoadSimulationFromMetaData(Database_Models.SimulationMetaData simMetaData)
    {
        algorithmName = simMetaData.algorithmName;
        NNModel model = OnnxDataToNNModel(modelData);
        Debug.Log("Loaded model with data");
        SetMLEnvironment(simMetaData.environmentName);
        SetModel(model, simMetaData.algorithmName);
        downloadedModelData = false;
    }


    void InitalizeEnvironment()
    {
        GameObject env = Instantiate(MLenvironment);
        env.transform.position = Vector3.zero;
        agent = env.GetComponentInChildren<AgentWrapper>();
    }

    void InitializeAgent()
    {
        agent.SetModel(algorithmName, model);
        print($"Testing model: {algorithmName}");
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
        algorithmName = name;
        InitializeAgent();
    }

    NNModel ModelToNNModel(Model model)
    {
        NNModelData modelData = ScriptableObject.CreateInstance<NNModelData>();
        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            ModelWriter.Save(writer, model);
            modelData.Value = memoryStream.ToArray();
        }
        modelData.name = "Data";
        modelData.hideFlags = HideFlags.HideInHierarchy;

        NNModel nnModel = ScriptableObject.CreateInstance<NNModel>();
        nnModel.modelData = modelData;
        return nnModel;
    }

    NNModel OnnxDataToNNModel(byte[] data)
    {
        ONNXModelConverter onnx = new ONNXModelConverter(true, false, true);
        //string path = SaveLoad<byte[]>.Save(data, "", algorithmName);
        Model model = onnx.Convert(data);
        NNModel nnModel = ModelToNNModel(model);
        nnModel.name = algorithmName;
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
