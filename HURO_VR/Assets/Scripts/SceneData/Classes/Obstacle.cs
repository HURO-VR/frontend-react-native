using UnityEngine;
using System.Collections.Generic;
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
        length = renderer.bounds.size.z;
        width = renderer.bounds.size.x;
        this.radius = Mathf.Max(width, length) / 2f;
        this.go = go;
        LoadCircleAbstraction();
        if (circleAbstraction != null) Debug.Log(go.name + " generated " + circleAbstraction.Count + " circles.");
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