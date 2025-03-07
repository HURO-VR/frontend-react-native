using System.Collections.Generic;

public static class SimulationRunConverter
{
    public static Dictionary<string, object> ToDictionary(SimulationRun simRun)
    {
        return new Dictionary<string, object>
        {
            { "uid", simRun.uid },
            { "dateCreated", simRun.dateCreated },
            { "status", simRun.status },
            { "simID", simRun.simID },
            { "starred", simRun.starred },
            { "runID", simRun.runID },
            { "name", simRun.name },
            { "data", simRun.data != null ? SimulationRunDataToDictionary(simRun.data) : null },
            { "errorMessage", simRun.errorMessage}
        };
    }

    private static Dictionary<string, object> SimulationRunDataToDictionary(SimulationRunData data)
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
