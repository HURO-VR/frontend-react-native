public class Circle
{
    public float radius;
    public XYZ position;

    /// <summary>
    /// Initializes a Circle instance with a specified position and radius.
    /// </summary>
    /// <param name="pos">The position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    public Circle(XYZ pos, float radius)
    {
        this.position.x = pos.x;
        this.position.y = pos.y;
        this.position.z = pos.z;
        this.radius = radius;
    }
}