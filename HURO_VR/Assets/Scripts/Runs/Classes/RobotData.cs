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
