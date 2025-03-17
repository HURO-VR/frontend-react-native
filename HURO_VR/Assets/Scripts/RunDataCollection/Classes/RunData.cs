using System.Collections.Generic;

/// <summary>
/// Represents the simulation run data including time, collisions, and entity-specific data.
/// </summary>
public class RunData
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the total time to complete the run in milliseconds.
    /// </summary>
    public int timeToComplete { get; set; } // Time in milliseconds

    /// <summary>
    /// Gets or sets the list of total collision points recorded during the run.
    /// </summary>
    public List<XYZ> totalCollisions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the simulation encountered a deadlock.
    /// </summary>
    public bool deadlock { get; set; }

    /// <summary>
    /// Gets or sets the list of robot-specific data for the simulation.
    /// </summary>
    public List<RobotData> robotData { get; set; }

    /// <summary>
    /// Gets or sets the list of obstacle-specific data present in the simulation.
    /// </summary>
    public List<ObstacleData> obstacleData { get; set; }

    #endregion

    #region Public Fields
    // (Add public fields here if needed in the future.)
    #endregion
}
