using System.Collections.Generic;

public class RobotData
{
    public XYZ robotStart { get; set; }
    public XYZ robotEnd { get; set; }
    public List<XYZ> robotPath { get; set; }
    public string name { get; set; }
    public int timeToComplete { get; set; } // Time in milliseconds
    public List<XYZ> collisions { get; set; }
    public XYZ goalPosition { get; set; }
    public bool goalReached { get; set; }
}

public class ObstacleData
{
    public XYZ position { get; set; }
    public float radius { get; set; }
}

public class SimulationRunData
{
    public int timeToComplete { get; set; } // Time in milliseconds
    public List<XYZ> totalCollisions { get; set; }
    public bool deadlock { get; set; }
    public List<RobotData> robotData { get; set; }
    public List<ObstacleData> obstacleData { get; set; }
}

public partial class SimulationRun
{
    public string uid { get; set; }
    public string dateCreated { get; set; }
    public string status { get; set; }
    public string simID { get; set; }
    public bool starred { get; set; }
    public string runID { get; set; }
    public string name { get; set; }

    public int serverHits = 0;

    public string errorMessage { get; set; }
    public SimulationRunData data { get; set; }

}

public class RunStatus
{
    public static string Failed = "failed";
    public static string Warning = "warning";
    public static string success = "success";
}