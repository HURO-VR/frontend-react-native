/// <summary>
/// Contains metadata for a simulation, including identifiers, algorithm file details, and environment settings.
/// </summary>
public struct SimulationMetaData
{
    #region Public Fields

    /// <summary>
    /// User defined name for the simulation.
    /// </summary>
    public string name;

    /// <summary>
    /// Simulation ID (e.g., the folder name for the simulation in the storage bucket).
    /// </summary>
    public string ID;

    /// <summary>
    /// The date the simulation was created in ISO format (e.g., 2025-02-02T12:34:56.789Z).
    /// </summary>
    public string dateCreated;

    /// <summary>
    /// The filename of the algorithm used for the simulation.
    /// </summary>
    public string algorithmFilename;

    /// <summary>
    /// Name of the training environment to load.
    /// </summary>
    public string environmentName;

    #endregion
}
