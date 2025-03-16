using UnityEngine;

public class Robot : Entity
{
    public XYZ position;
    public XYZ goal;
    public float goal_radius;
    public XYZ curr_velocity;
    public float max_velocity;
    public float radius;
    public string name;

    float DEFAULT_GOAL_RADIUS = 0.05f;

    public Robot(GameObject go)
    {
        SetData(go);
    }

    public void SetData(GameObject go)
    {
        Transform robot_transform = go.transform;
        Vector3 robot_velocity = go.GetComponent<Rigidbody>().velocity;
        RobotEntity robotController = go.GetComponent<RobotEntity>();
        SphereCollider sphereCollider = go.GetComponent<SphereCollider>();
        

        Transform goal_transform = robotController.GetGoal().transform;
        Collider goalCollider = goal_transform.GetComponent<Collider>();

        Renderer renderer = robot_transform.GetComponent<Renderer>();
        this.name = go.name;

        this.position.x = robot_transform.position.x;
        this.position.y = robot_transform.position.y;
        this.position.z = robot_transform.position.z;

        this.curr_velocity.x = robot_velocity.x;
        this.curr_velocity.y = robot_velocity.y;
        this.curr_velocity.z = robot_velocity.z;

        this.goal.x = goal_transform.position.x;
        this.goal.y = goal_transform.position.y;
        this.goal.z = goal_transform.position.z;

        this.max_velocity = robotController.maxVelocity;
        this.radius = sphereCollider.radius * robot_transform.localScale.x;
        this.goal_radius = DEFAULT_GOAL_RADIUS;
    }

    public void DrawGizmo()
    {
        if (radius == 0) return;
        SceneDataUtils.DrawCircleGizmo(position, radius, Color.red);
        SceneDataUtils.DrawCircleGizmo(goal, goal_radius, Color.red);
    }
}