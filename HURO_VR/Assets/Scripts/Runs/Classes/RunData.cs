using System.Collections.Generic;

public class RunData
{
    public int timeToComplete { get; set; } // Time in milliseconds
    public List<XYZ> totalCollisions { get; set; }
    public bool deadlock { get; set; }
    public List<RobotData> robotData { get; set; }
    public List<ObstacleData> obstacleData { get; set; }
}
