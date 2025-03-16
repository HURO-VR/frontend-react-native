using System.Collections.Generic;

public partial class RunMetadata
{
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

    private static Dictionary<string, object> ObstacleDataToDictionary(ObstacleData obstacle)
    {
        return new Dictionary<string, object>
        {
            { "position", XYZToDictionary(obstacle.position) },
            { "radius", obstacle.radius }
        };
    }

    private static Dictionary<string, object> XYZToDictionary(XYZ xyz)
    {
        return new Dictionary<string, object>
        {
            { "x", xyz.x },
            { "y", xyz.y },
            { "z", xyz.z }
        };
    }
}
