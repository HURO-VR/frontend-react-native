public partial class RunMetadata
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the unique identifier for this run metadata.
    /// </summary>
    public string uid { get; set; }

    /// <summary>
    /// Gets or sets the date when the run was created, formatted as a string.
    /// </summary>
    public string dateCreated { get; set; }

    /// <summary>
    /// Gets or sets the current status of the run (e.g., success, warning, failed).
    /// </summary>
    public string status { get; set; }

    /// <summary>
    /// Gets or sets the simulation ID associated with this run.
    /// </summary>
    public string simID { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this run is starred.
    /// </summary>
    public bool starred { get; set; }

    /// <summary>
    /// Gets or sets the run ID.
    /// </summary>
    public string runID { get; set; }

    /// <summary>
    /// Gets or sets the name of the run.
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the error message associated with this run, if any.
    /// </summary>
    public string errorMessage { get; set; }

    /// <summary>
    /// Gets or sets the simulation run data.
    /// </summary>
    public RunData data { get; set; }

    /// <summary>
    /// The number of server hits recorded during the run.
    /// </summary>
    public int serverHits = 0;

    #endregion
}
