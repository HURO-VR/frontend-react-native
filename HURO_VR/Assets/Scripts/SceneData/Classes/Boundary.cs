using UnityEngine;

public class Boundary : Entity
{
    public XYZ position;
    public float width;
    public float length;

    public void DrawGizmo()
    {
        SceneDataUtils.DrawRectangleGizmo(position, width, length);
    }

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