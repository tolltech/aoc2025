namespace AoC_2025;

public readonly record struct Point3D(int x, int y, int z)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public int Z { get; } = z;

    public override string ToString() => $"({X}, {Y}, {Z})";

    // Common operations
    public static Point3D operator +(Point3D a, Point3D b) =>
        new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Point3D operator -(Point3D a, Point3D b) =>
        new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Point3D operator *(Point3D point, int scalar) =>
        new Point3D(point.X * scalar, point.Y * scalar, point.Z * scalar);

    public static Point3D operator /(Point3D point, int scalar) =>
        new Point3D(point.X / scalar, point.Y / scalar, point.Z / scalar);

    public double DistanceTo(Point3D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        double dz = Z - other.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static Point3D Zero => new Point3D(0, 0, 0);
    public static Point3D One => new Point3D(1, 1, 1);
    public static Point3D UnitX => new Point3D(1, 0, 0);
    public static Point3D UnitY => new Point3D(0, 1, 0);
    public static Point3D UnitZ => new Point3D(0, 0, 1);
}