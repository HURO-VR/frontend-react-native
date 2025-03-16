using System.Collections.Generic;
using UnityEngine;

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




 



