using System.Collections.Generic;
using UnityEngine;

public struct XYZ
{
    public float x;
    public float y;
    public float z;

    public XYZ(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class Robot
{
    public XYZ position;
    public XYZ goal;
    public XYZ curr_velocity;
    public float max_velocity;
    public float radius;
    public string name;

    public void SetData(GameObject go)
    {
        Transform robot_transform = go.transform;
        Vector3 robot_velocity = go.GetComponent<Rigidbody>().velocity;
        RobotController robotController = go.GetComponent<RobotController>();
        SphereCollider sphereCollider = go.GetComponent<SphereCollider>();

        Transform goal_transform = robotController.goal.transform;
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
        this.radius = renderer.bounds.size.x / 2;
    }
}

public class Boundary
{
    public XYZ position;
    public float width;
    public float length;
}
 

public class Obstacle
{
    //static float ABSTRACTION_RADIUS = 0.1f;
    //static float squareThreshold = 0.1f;
    public XYZ position;
    public float radius;
    public List<Obstacle> circleAbstraction;
    public bool isDynamic;

    public void SetData(GameObject go)
    {
        this.position.x = go.transform.position.x;
        this.position.y = go.transform.position.y;
        this.position.z = go.transform.position.z;
        this.isDynamic = false;

        Renderer renderer = go.GetComponent<Renderer>();
        this.radius = (renderer.bounds.size.z + renderer.bounds.size.x) / 2;

        // If object is close to a square, represent as a circle.
        //bool shouldAbstract = !(renderer.bounds.size.z < renderer.bounds.size.x + squareThreshold && renderer.bounds.size.z > renderer.bounds.size.x - squareThreshold);
        //if (shouldAbstract) circleAbstraction = GeneratePerimeterCircles(renderer, ABSTRACTION_RADIUS);
    }

    static List<Obstacle> GeneratePerimeterCircles(Renderer renderer, float radius)
    {
        List<Obstacle> circles = new List<Obstacle>();

        Vector3 size = renderer.bounds.size;
        Vector3 center = renderer.bounds.center;

        float halfX = size.x / 2f;
        float halfZ = size.z / 2f;

        List<XYZ> perimeterPoints = new List<XYZ>();

        // Generate points along the perimeter at intervals of radius * 2 (for spacing)
        for (float x = -halfX; x <= halfX; x += radius * 2)
        {
            perimeterPoints.Add(new XYZ(center.x + x, center.y, center.z + halfZ)); // Top edge
            perimeterPoints.Add(new XYZ(center.x + x, center.y, center.z - halfZ)); // Bottom edge
        }
        for (float z = -halfZ; z <= halfZ; z += radius * 2)
        {
            perimeterPoints.Add(new XYZ(center.x + halfX, center.y, center.z + z)); // Right edge
            perimeterPoints.Add(new XYZ(center.x - halfX, center.y, center.z + z)); // Left edge
        }

        // Convert to circle representation
        foreach (var point in perimeterPoints)
        {
            Obstacle obstacle = new()
            {
                position = point,
                radius = radius
            };
            circles.Add(obstacle);
        }

        return circles;
    }

    public static Obstacle[] UnpackAbstractions(Obstacle[] obstacles)
    {
        Debug.Log("Unpacking " + obstacles.Length + " obstacles.");
        List<Obstacle> list = new List<Obstacle>();
        foreach (var obstacle in obstacles)
        {
            if (obstacle.circleAbstraction == null)
            {
                list.Add(obstacle);
            }
            else foreach (var circle in obstacle.circleAbstraction)
            {
                list.Add(circle);
            }
        }
        return list.ToArray();
    }


}

public class SceneData
{
    public float robot_radius;
    public Obstacle[] obstacles;
    public Boundary boundary;
    public Robot[] robots;
}
