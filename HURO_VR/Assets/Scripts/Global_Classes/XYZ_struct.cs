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

    public override string ToString()
    {
        return $"(X: {x}, Y: {y}, Z: {z})";
    }

    public static XYZ operator +(XYZ a, XYZ b)
    {
        return new XYZ(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static implicit operator XYZ(Vector3 v)
    {
        return new XYZ(v.x, v.y, v.z);
    }

    public static implicit operator Vector3(XYZ xyz)
    {
        return new Vector3(xyz.x, xyz.y, xyz.z);
    }
}