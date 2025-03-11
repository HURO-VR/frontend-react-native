using System.Collections.Generic;
using UnityEngine;
using static IronPython.Modules._ast;
using UnityEngine.UIElements;

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

    public override string ToString()
    {
        return $"(X: {x}, Y: {y}, Z: {z})";
    }
}
public class Circle
{
    public float radius;
    public XYZ position;

    public Circle(XYZ pos, float radius)
    {
        this.position.x = pos.x;
        this.position.y = pos.y;
        this.position.z = pos.z;
        this.radius = radius;
    }
}

    public class DataUtils {
    public static void DrawCircleGizmo(XYZ position, float radius, Color color)
    {
        Gizmos.color = color;
        int lineSegments = 32;
        Vector3 previousPoint = Vector3.zero;
        for (int i = 0; i <= lineSegments; i++)
        {
            float angle = i * Mathf.PI * 2 / lineSegments;
            Vector3 newPoint = new Vector3(
                position.x + Mathf.Cos(angle) * radius,
                position.y,
                position.z + Mathf.Sin(angle) * radius
            );

            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, newPoint);
            }
            previousPoint = newPoint;
        }
    }

    public static void DrawRectangleGizmo(XYZ position, float width, float length)
    {
        Vector3 p1 = new Vector3(position.x - width / 2, position.y, position.z - length / 2);
        Vector3 p2 = new Vector3(position.x + width / 2, position.y, position.z - length / 2);
        Vector3 p3 = new Vector3(position.x + width / 2, position.y, position.z + length / 2);
        Vector3 p4 = new Vector3(position.x - width / 2, position.y, position.z + length / 2);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    static XYZ RotatePoint(XYZ point, XYZ origin, float angleDeg)
    {
        float angleRad = Mathf.PI * angleDeg / 180.0f * -1;
        float cosA = Mathf.Cos(angleRad);
        float sinA = Mathf.Sin(angleRad);

        // Translate point to origin
        float translatedX = point.x - origin.x;
        float translatedY = point.z - origin.z;

        // Rotate the point
        float rotatedX = translatedX * cosA - translatedY * sinA;
        float rotatedY = translatedX * sinA + translatedY * cosA;

        // Translate back
        point.x = rotatedX + origin.x;
        point.z = rotatedY + origin.z;
        return point;
    }

    public static List<Circle> GenerateCircles(XYZ position, float width, float length, float rotation)
    {
        List<Circle> circles = new List<Circle>();

        float rad = Mathf.Min(width, length) / 20f;
        int cols = Mathf.CeilToInt(width / rad / 2);
        int rows = Mathf.CeilToInt(length / rad / 2);

        for (int i = 0; i < cols + 1; i++)
        {
            for (int j = 0; j < rows + 1; j++)
            {
                if (i == 0 || i == cols || j == rows || j == 0)
                {
                    float x = ((rad * 2) * i) - (width / 2);
                    float z = ((rad * 2) * j) - (length / 2);
                    XYZ pos = new XYZ(position.x + x, position.y, position.z + z);
                    pos = RotatePoint(pos, position, rotation);
                    circles.Add(new Circle(pos, rad));
                }
                
            }
        }

        return circles;
    }
}


public class Robot
{
    public XYZ position;
    public XYZ goal;
    public float goal_radius;
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
        

        Transform goal_transform = robotController.GetGoal().transform;
        SphereCollider goalCollider = goal_transform.GetComponent<SphereCollider>();

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
        this.goal_radius = goalCollider.radius * goal_transform.localScale.x;
    }

    public void DrawGizmo()
    {
        if (radius == 0) return;
        DataUtils.DrawCircleGizmo(position, radius, Color.red);
        DataUtils.DrawCircleGizmo(goal, goal_radius, Color.red);
    }
}

public class Boundary
{
    public XYZ position;
    public float width;
    public float length;

    public void DrawGizmo()
    {
        DataUtils.DrawRectangleGizmo(position, width, length);
    }
}
 

public class Obstacle : Circle
{
    //static float ABSTRACTION_RADIUS = 0.1f;
    //static float squareThreshold = 0.1f;
    public List<Circle> circleAbstraction;
    public float width;
    public float length;
    public bool isDynamic;
    public GameObject go;

    public Obstacle(GameObject go) : base(new XYZ(go.transform.position.x, go.transform.position.y, go.transform.position.z), 0f)
    {
        this.isDynamic = go.name.ToLower().Contains("user");
        Renderer renderer = go.GetComponent<Renderer>();
        float length = renderer.bounds.size.z;
        float width = renderer.bounds.size.x;
        this.radius = Mathf.Max(width, length) / 2f;
        LoadCircleAbstraction();
        if (circleAbstraction != null) Debug.Log(go.name + " generated " + circleAbstraction.Count + " circles.");
        this.go = go;
    }
    public void LoadCircleAbstraction()
    {
        if (!(width < length + .2f && width > length - .2f)) 
            circleAbstraction = DataUtils.GenerateCircles(position, width, length, go.transform.eulerAngles.y);
    }

    public Circle ToCircle()
    {
        return new Circle(position, radius);
    }

    public Circle[] ToCircles()
    {
        return circleAbstraction.ToArray();
    }

    public void DrawGizmo()
    {
        if (radius == 0) return;
        if (circleAbstraction != null && circleAbstraction.Count > 0)
        {
            foreach (var circle in circleAbstraction)
            {
                DataUtils.DrawCircleGizmo(circle.position, circle.radius, Color.red);
            }
        }
        else DataUtils.DrawCircleGizmo(position, radius, Color.red);
    }

    public static Circle[] UnpackAbstractions(Obstacle[] obstacles)
    {
        List<Circle> list = new List<Circle>();
        foreach (var obstacle in obstacles)
        {
            if (obstacle.circleAbstraction != null && obstacle.circleAbstraction.Count > 0)
            {
                foreach (var circle in obstacle.circleAbstraction)
                {
                    list.Add(circle);
                }
            }
            else
            {
                list.Add(obstacle.ToCircle());
            }
        }
        Debug.Log("Unpacked " + obstacles.Length + " obstacles into " + list.Count + " circles.");

        return list.ToArray();
    }
}

public class SceneData
{
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

public struct SceneDataOutput
{
    public float robot_radius;
    public Circle[] obstacles;
    public Boundary boundary;
    public Robot[] robots;
}
