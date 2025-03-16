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