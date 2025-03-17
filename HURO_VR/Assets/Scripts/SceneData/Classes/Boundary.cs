using UnityEngine;

public class Boundary : Entity
{
    public XYZ position;
    public float width;
    public float length;

    /// <summary>
    /// Draws a rectangle gizmo representing the boundary in the scene view.
    /// </summary>
    public void DrawGizmo()
    {
        SceneDataUtils.DrawRectangleGizmo(position, width, length);
    }

    /// <summary>
    /// Initializes a boundary using the dimensions of the provided floor GameObject.
    /// </summary>
    /// <param name="floor">The GameObject representing the floor from which boundary dimensions are derived.</param>
    public Boundary(GameObject floor)
    {
        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Scene floor not detected. No boundary set.");
            return;
        }

        position.x = floor.transform.position.x;
        position.y = floor.transform.position.y;
        position.z = floor.transform.position.z;

        width = renderer.bounds.size.x;
        length = renderer.bounds.size.z;
    }
}
