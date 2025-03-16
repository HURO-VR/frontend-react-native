using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an obstacle in the game world.
/// </summary>
public class Obstacle : Circle
{
    // Static constants
    // static float ABSTRACTION_RADIUS = 0.1f;
    // static float squareThreshold = 0.1f;

    /// <summary>
    /// List of circles that approximate the obstacle's shape.
    /// </summary>
    public List<Circle> circleAbstraction;

    /// <summary>
    /// Width of the obstacle.
    /// </summary>
    public float width;

    /// <summary>
    /// Length of the obstacle.
    /// </summary>
    public float length;

    /// <summary>
    /// Whether the obstacle is dynamic (i.e., can move).
    /// </summary>
    public bool isDynamic;

    /// <summary>
    /// The GameObject associated with this obstacle.
    /// </summary>
    public GameObject go;

    /// <summary>
    /// Initializes a new Obstacle instance.
    /// </summary>
    /// <param name="go">The GameObject to associate with this obstacle.</param>
    public Obstacle(GameObject go) : base(new XYZ(go.transform.position.x, go.transform.position.y, go.transform.position.z), 0f)
    {
        this.isDynamic = go.name.ToLower().Contains("user");
        Renderer renderer = go.GetComponent<Renderer>();
        length = renderer.bounds.size.z;
        width = renderer.bounds.size.x;
        this.radius = Mathf.Max(width, length) / 2f;
        this.go = go;
        CreateCircleAbstraction();
        if (circleAbstraction != null) Debug.Log(go.name + " generated " + circleAbstraction.Count + " circles.");
    }

    /// <summary>
    /// Creates the circle abstraction for this obstacle.
    /// </summary>
    public void CreateCircleAbstraction()
    {
        if (!(width < length + .2f && width > length - .2f)) 
            circleAbstraction = DataUtils.GenerateCircles(position, width, length, go.transform.eulerAngles.y);
    }

    /// <summary>
    /// Converts this obstacle to a Circle instance.
    /// </summary>
    /// <returns>A Circle instance representing this obstacle.</returns>
    public Circle ToCircle()
    {
        return new Circle(position, radius);
    }

    /// <summary>
    /// Draws a gizmo for this obstacle in the Unity editor.
    /// </summary>
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

    /// <summary>
    /// Unpacks the circle abstractions from an array of obstacles.
    /// </summary>
    /// <param name="obstacles">The array of obstacles to unpack.</param>
    /// <returns>An array of Circle instances representing the unpacked abstractions.</returns>
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