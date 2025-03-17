using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.Policies;
using UnityEditor;
using Unity.Barracuda;
using Unity.Barracuda.ONNX;
using System.IO;

/// <summary>
/// Manages the simulation using ONNX models, including model downloading, environment setup, and running the simulation.
/// </summary>
public class ONNXSimulationManager : MonoBehaviour
{
    #region Private Fields

    /// <summary>
    /// The agent wrapper controlling the simulation agent.
    /// </summary>
    private AgentWrapper agent;

    /// <summary>
    /// The NNModel used for inference during the simulation.
    /// </summary>
    private NNModel model;

    /// <summary>
    /// The storage service for downloading simulation data.
    /// </summary>
    private Storage storage;

    /// <summary>
    /// The downloaded model data as a byte array.
    /// </summary>
    private byte[] modelData;

    /// <summary>
    /// The currently selected simulation metadata bundle.
    /// </summary>
    private SimulationMetaData selectedSimulation;

    /// <summary>
    /// Flag indicating whether the model data has been downloaded.
    /// </summary>
    private bool downloadedModelData = false;

    /// <summary>
    /// Flag indicating whether the simulation is currently running metadata.
    /// </summary>
    private bool runMetadataning = false;

    /// <summary>
    /// Flag indicating whether the simulation should run.
    /// </summary>
    private bool runSimulation = false;

    #endregion

    #region Public Fields

    /// <summary>
    /// The Machine Learning environment GameObject.
    /// </summary>
    public GameObject MLenvironment;

    /// <summary>
    /// The number of simulation runs per model.
    /// </summary>
    public int RunsPerModel = 100;

    /// <summary>
    /// The model template.
    /// </summary>
    public NNModel model_template;

    /// <summary>
    /// An array of available simulation metadata bundles.
    /// </summary>
    public SimulationMetaData[] simMetaDatas = new SimulationMetaData[0];

    /// <summary>
    /// Gets a value indicating whether simulation metadata has been received.
    /// </summary>
    public bool recievedMetaData { get; private set; }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called when the script instance is being loaded. Initializes the storage component.
    /// </summary>
    public void Awake()
    {
        storage = FindAnyObjectByType<Storage>();
        if (storage == null)
        {
            storage = gameObject.AddComponent<Storage>();
        }
        recievedMetaData = false;
    }

    /// <summary>
    /// Called on the frame when the script is enabled. Initializes simulation metadata.
    /// </summary>
    private void Start()
    {
        Debug.Log("Running ModelTester.");
        InitSimulationMetaData();
    }

    /// <summary>
    /// Called once per frame to update simulation status and handle user input.
    /// </summary>
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

        if (selectedSimulation.name != null && downloadedModelData && !runMetadataning && runSimulation)
        {
            runMetadataning = true;
            Debug.Log("Loading sim bundle " + simMetaDatas[0].name);
            _RunSimulation();
        }
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Sets the selected simulation bundle and downloads the corresponding model data.
    /// </summary>
    /// <param name="bundle">The simulation metadata bundle to select.</param>
    void SelectSimulation(SimulationMetaData bundle)
    {
        selectedSimulation = bundle;
        storage.DownloadFile(simMetaDatas[0].algorithmFilename, FileType.Algorithm, simMetaDatas[0].ID, (data) =>
        {
            modelData = data;
            downloadedModelData = true;
            Debug.Log("Downloaded model data");
        });
    }

    /// <summary>
    /// Flags the simulation to run.
    /// </summary>
    public void RunSimulation()
    {
        runSimulation = true;
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Runs the selected simulation metadata bundle by converting downloaded model data into an NNModel,
    /// setting up the ML environment, and initializing the model for the agent.
    /// </summary>
    void _RunSimulation()
    {
        NNModel model = OnnxDataToNNModel(modelData);
        Debug.Log("Loaded model with data");
        SetMLEnvironment(selectedSimulation.environmentName);
        SetModel(model, selectedSimulation.algorithmFilename);
        downloadedModelData = false;
    }

    /// <summary>
    /// Asynchronously retrieves all available simulation metadata bundles and stores them in the global array.
    /// </summary>
    async void InitSimulationMetaData()
    {
        Debug.Log("Getting sim bundles");
        await storage.GetAllSimulationBundles((data) =>
        {
            simMetaDatas = data;
            Debug.Log("Returned " + simMetaDatas.Length + " bundles");
            recievedMetaData = true;
        });
    }

    /// <summary>
    /// Instantiates the ML environment from the assigned prefab and sets the agent reference.
    /// </summary>
    void InitalizeEnvironment()
    {
        GameObject env = Instantiate(MLenvironment);
        env.transform.position = Vector3.zero;
        agent = env.GetComponentInChildren<AgentWrapper>();
    }

    /// <summary>
    /// Initializes the agent for testing by setting its model, behavior type, and maximum run count.
    /// </summary>
    void InitializeAgent()
    {
        agent.SetModel(selectedSimulation.algorithmFilename, model);
        Debug.Log($"Testing model: {selectedSimulation.algorithmFilename}");
        agent.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
        agent.maxRuns = RunsPerModel;
        agent.SetTesting(true);
    }

    /// <summary>
    /// Sets up the ML environment based on the given environment name.
    /// </summary>
    /// <param name="environmentName">The name of the environment to load.</param>
    void SetMLEnvironment(string environmentName)
    {
        // Overriding the environment name as per the current configuration.
        environmentName = "Empty_Room";
        MLenvironment = Resources.Load<GameObject>($"Training_Environments/{environmentName}");
        InitalizeEnvironment();
    }

    /// <summary>
    /// Sets the current NNModel and initializes the agent with the new model.
    /// </summary>
    /// <param name="_model">The NNModel to set.</param>
    /// <param name="name">The name of the algorithm file associated with the model.</param>
    void SetModel(NNModel _model, string name)
    {
        model = _model;
        InitializeAgent();
    }

    /// <summary>
    /// Converts a Barracuda Model to an NNModel.
    /// </summary>
    /// <param name="model">The Barracuda Model to convert.</param>
    /// <returns>An NNModel containing the converted model data.</returns>
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

    /// <summary>
    /// Converts ONNX model data into an NNModel.
    /// </summary>
    /// <param name="data">The byte array containing the ONNX model data.</param>
    /// <returns>The NNModel generated from the ONNX data.</returns>
    NNModel OnnxDataToNNModel(byte[] data)
    {
        ONNXModelConverter onnx = new ONNXModelConverter(true, false, true);
        Model model = onnx.Convert(data);
        NNModel nnModel = ModelToNNModel(model);
        nnModel.name = selectedSimulation.algorithmFilename;
        return nnModel;
    }

    #endregion
}

/// <summary>
/// Wraps an Agent to provide additional functionality for logging and statistics during simulation runs.
/// </summary>
public class AgentWrapper : Agent
{
    #region Private Fields

    /// <summary>
    /// The number of failed runs.
    /// </summary>
    private float numFails;

    /// <summary>
    /// The number of successful runs.
    /// </summary>
    private float numSuccesses;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets a value indicating whether the agent is in testing mode.
    /// </summary>
    public bool _testing { get; private set; }

    /// <summary>
    /// Gets the number of simulation runs that have been executed.
    /// </summary>
    public float numRuns { get; private set; }

    /// <summary>
    /// Gets or sets the maximum number of runs for the simulation.
    /// </summary>
    public int maxRuns { private get; set; }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Called when the agent is started. Initializes run statistics.
    /// </summary>
    public void Start()
    {
        numFails = 0;
        numSuccesses = 0;
        numRuns = 0;
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Logs a failed simulation run.
    /// </summary>
    public void LogFail()
    {
        numFails++;
    }

    /// <summary>
    /// Logs a successful simulation run.
    /// </summary>
    public void LogSuccess()
    {
        numSuccesses++;
    }

    /// <summary>
    /// Logs a simulation run.
    /// </summary>
    public void LogRun()
    {
        numRuns++;
    }

    /// <summary>
    /// Sets the testing mode for the agent.
    /// </summary>
    /// <param name="test">If set to <c>true</c> enables testing mode; otherwise, disables it.</param>
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

    /// <summary>
    /// Retrieves the simulation statistics including success, failure, and deadlock percentages.
    /// </summary>
    /// <returns>A SimulationStatistics structure containing the percentages.</returns>
    public SimulationStatistics GetStats()
    {
        SimulationStatistics stats = new SimulationStatistics();
        stats.percentSuccess = (numSuccesses / numRuns) * 100;
        stats.percentFailure = (numFails / numRuns) * 100;
        stats.percentDeadlock = (Mathf.Abs(numSuccesses + numFails - numRuns) / numRuns) * 100;
        return stats;
    }

    /// <summary>
    /// Prints the simulation statistics to the console.
    /// </summary>
    public void PrintStats()
    {
        SimulationStatistics stats = GetStats();
        Debug.Log("Percentage success: " + stats.percentSuccess);
        Debug.Log("Percentage failed: " + stats.percentFailure);
        Debug.Log("Percentage deadlock: " + stats.percentDeadlock);
    }

    /// <summary>
    /// Resets the simulation statistics.
    /// </summary>
    public void ResetStats()
    {
        numFails = 0;
        numSuccesses = 0;
        numRuns = 0;
    }

    #endregion

    #region Public Structures

    /// <summary>
    /// Structure holding simulation statistics.
    /// </summary>
    public struct SimulationStatistics
    {
        /// <summary>
        /// The percentage of successful runs.
        /// </summary>
        public float percentSuccess;

        /// <summary>
        /// The percentage of failed runs.
        /// </summary>
        public float percentFailure;

        /// <summary>
        /// The percentage of runs that resulted in deadlock.
        /// </summary>
        public float percentDeadlock;
    }

    #endregion
}
