using System.Collections.Generic;

public partial class RunMetadata
{
    #region Public Functions

    /// <summary>
    /// Converts the current RunMetadata object to a dictionary.
    /// </summary>
    /// <returns>A dictionary representation of the RunMetadata.</returns>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "uid", this.uid },
            { "dateCreated", this.dateCreated },
            { "status", this.status },
            { "simID", this.simID },
            { "starred", this.starred },
            { "runID", this.runID },
            { "name", this.name },
            { "data", this.data != null ? SimulationRunDataToDictionary(this.data) : null },
            { "errorMessage", this.errorMessage },
            { "serverHits", this.serverHits }
        };
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Converts the simulation run data to a dictionary.
    /// </summary>
    /// <param name="data">The simulation run data.</param>
    /// <returns>A dictionary representation of the RunData.</returns>
    private static Dictionary<string, object> SimulationRunDataToDictionary(RunData data)
    {
        return new Dictionary<string, object>
        {
            { "timeToComplete", data.timeToComplete },
            { "totalCollisions", data.totalCollisions?.ConvertAll(XYZToDictionary) },
            { "deadlock", data.deadlock },
            { "robotData", data.robotData?.ConvertAll(RobotDataToDictionary) },
            { "obstacleData", data.obstacleData?.ConvertAll(ObstacleDataToDictionary) }
        };
    }

    /// <summary>
    /// Converts the robot data to a dictionary.
    /// </summary>
    /// <param name="robot">The robot data.</param>
    /// <returns>A dictionary representation of the RobotData.</returns>
    private static Dictionary<string, object> RobotDataToDictionary(RobotData robot)
    {
        return new Dictionary<string, object>
        {
            { "robotStart", XYZToDictionary(robot.robotStart) },
            { "robotEnd", XYZToDictionary(robot.robotEnd) },
            { "robotPath", robot.robotPath?.ConvertAll(XYZToDictionary) },
            { "name", robot.name },
            { "timeToComplete", robot.timeToComplete },
            { "collisions", robot.collisions?.ConvertAll(XYZToDictionary) },
            { "goalPosition", XYZToDictionary(robot.goalPosition) },
            { "goalReached", robot.goalReached }
        };
    }

    /// <summary>
    /// Converts the obstacle data to a dictionary.
    /// </summary>
    /// <param name="obstacle">The obstacle data.</param>
    /// <returns>A dictionary representation of the ObstacleData.</returns>
    private static Dictionary<string, object> ObstacleDataToDictionary(ObstacleData obstacle)
    {
        return new Dictionary<string, object>
        {
            { "position", XYZToDictionary(obstacle.position) },
            { "radius", obstacle.radius }
        };
    }

    /// <summary>
    /// Converts an XYZ object to a dictionary.
    /// </summary>
    /// <param name="xyz">The XYZ object.</param>
    /// <returns>A dictionary with keys "x", "y", and "z" corresponding to the XYZ values.</returns>
    private static Dictionary<string, object> XYZToDictionary(XYZ xyz)
    {
        return new Dictionary<string, object>
        {
            { "x", xyz.x },
            { "y", xyz.y },
            { "z", xyz.z }
        };
    }

    #endregion
}
