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