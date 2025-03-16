public class SceneData
{
    public struct SceneDataOutput
    {
        public float robot_radius;
        public Circle[] obstacles;
        public Boundary boundary;
        public Robot[] robots;
    }

    public float robot_radius;
    public Obstacle[] fullObstacles;
    public Circle[] obstacles;
    public Boundary boundary;
    public Robot[] robots;
    private SceneDataOutput output;

    public SceneDataOutput LoadOutput()
    {
        output.robot_radius = robot_radius;
        output.robots = robots;
        output.boundary = boundary;
        output.obstacles = obstacles;
        return output;
    }
}


