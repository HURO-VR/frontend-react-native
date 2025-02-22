public struct XYZ
{
    public float x;
    public float y;
    public float z;
}

public class Robot
{
    public XYZ position;
    public XYZ goal;
    public XYZ curr_velocity;
    public float max_velocity;
    public float radius;
    public string name;
}

public class Boundary
{
    public XYZ position;
    public float width;
    public float length;
}

public class Obstacle
{
    public XYZ position;
    public float radius;
    public bool isDynamic;
}

public class SceneData
{
    public float robot_radius;
    public Obstacle[] obstacles;
    public Boundary boundary;
    public Robot[] robots;

}