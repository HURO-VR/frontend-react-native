using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Policies;
using UnityEditor;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;


public class ModelTester : MonoBehaviour
{
    private AgentWrapper agent;
    public GameObject MLenvironment;
    private NNModel model;
    public int RunsPerModel = 100;
    Storage storage;
    byte[] modelData;
    public NNModel model_template;
    public Database_Models.SimulationMetaData[] simMetaDatas = new Database_Models.SimulationMetaData[0];
    Database_Models.SimulationMetaData selectedSimulation;
    bool downloadedModelData = false;
    bool simulationRunning = false;
    public bool recievedMetaData { private set; get; }
    bool runSimulation = false;


    public void Awake()
    {
        storage = FindAnyObjectByType<Storage>();
        if (storage == null)
        {
            storage = gameObject.AddComponent<Storage>();
        }
        recievedMetaData = false;
    }

    private void Start()
    {
        Debug.Log("Running ModelTester.");
        InitSimulationMetaData();

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

        if (Input.GetKeyDown(KeyCode.Space) && recievedMetaData)
        {
            // TODO: Replace with user selected meta data bundle.
            SelectSimulation(simMetaDatas[0]);
            runSimulation = true;
        }

        if (selectedSimulation.name != null && downloadedModelData && !simulationRunning && runSimulation)
        {
            simulationRunning = true;
            Debug.Log("Loading sim bundle " + simMetaDatas[0].name);
            _RunSimulation();
        }

    }

    // PUBLIC FUNCTIONS

    /// <summary>
    /// Downloads necessary simulation data from metadata bundle.
    /// </summary>
    /// <param name="bundle">Bundle to load.</param>
    void SelectSimulation(Database_Models.SimulationMetaData bundle)
    {
        selectedSimulation = bundle;
        storage.DownloadFile(simMetaDatas[0].algorithmFilename, FileType.Algorithm, simMetaDatas[0].ID, (data) =>
        {
            modelData = data;
            downloadedModelData = true;
            Debug.Log("Downloaded model data");
        });
    }

    public void RunSimulation()
    {
        runSimulation = true;
    }



    /// <summary>
    /// Runs the selected meta data bundle.
    /// </summary>
    void _RunSimulation()
    {
        NNModel model = OnnxDataToNNModel(modelData);
        Debug.Log("Loaded model with data");
        SetMLEnvironment(selectedSimulation.environmentName);
        SetModel(model, selectedSimulation.algorithmFilename);
        downloadedModelData = false;
    }


    // PRIVATE FUNCTIONS

    /// <summary>
    /// Gets all simulation metadata and loads it into the global variable "simMetaDatas".4
    /// Used to display the list of selectable simulations.
    /// </summary>
    async void InitSimulationMetaData()
    {
        Debug.Log("Getting sim bundles");
        simMetaDatas = await storage.GetAllSimulationBundles();
        Debug.Log("Returned " + simMetaDatas.Length + " bundles");
        recievedMetaData = true;
    }



    void InitalizeEnvironment()
    {
        GameObject env = Instantiate(MLenvironment);
        env.transform.position = Vector3.zero;
        agent = env.GetComponentInChildren<AgentWrapper>();
    }

    void InitializeAgent()
    {
        agent.SetModel(selectedSimulation.algorithmFilename, model);
        print($"Testing model: {selectedSimulation.algorithmFilename}");
        agent.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
        agent.maxRuns = RunsPerModel;
        agent.SetTesting(true);
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
        Model model = onnx.Convert(data);
        NNModel nnModel = ModelToNNModel(model);
        nnModel.name = selectedSimulation.algorithmFilename;
        return nnModel;
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

    public void LogRun()
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
