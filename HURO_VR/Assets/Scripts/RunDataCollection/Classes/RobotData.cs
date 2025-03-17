using System.Collections.Generic;

/// <summary>
/// Represents the data associated with a single robot during a simulation run.
/// </summary>
public class RobotData
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the starting position of the robot.
    /// </summary>
    public XYZ robotStart { get; set; }

    /// <summary>
    /// Gets or sets the ending position of the robot.
    /// </summary>
    public XYZ robotEnd { get; set; }

    /// <summary>
    /// Gets or sets the path traversed by the robot.
    /// </summary>
    public List<XYZ> robotPath { get; set; }

    /// <summary>
    /// Gets or sets the name of the robot.
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the time taken for the robot to complete its task in milliseconds.
    /// </summary>
    public int timeToComplete { get; set; } // Time in milliseconds

    /// <summary>
    /// Gets or sets the list of collision points encountered by the robot.
    /// </summary>
    public List<XYZ> collisions { get; set; }

    /// <summary>
    /// Gets or sets the goal position that the robot is trying to reach.
    /// </summary>
    public XYZ goalPosition { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the robot has reached its goal.
    /// </summary>
    public bool goalReached { get; set; }

    #endregion

    #region Public Fields
    // (Add public fields here if needed in the future.)
    #endregion
}
