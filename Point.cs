using System.Diagnostics;

namespace AoC_2025;

[DebuggerDisplay("X:{X},Y:{Y}")]
public struct Point : IEquatable<Point>
{
    public Point()
    {
    }

    public Point(int Row, int Col)
    {
        this.Row = Row;
        this.Col = Col;
    }

    public int Col;
    public int Row;

    public int X
    {
        get => Col;
        set => Col = value;
    }

    public int Y
    {
        get => Row;
        set => Row = value;
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.Y + p2.Y, p1.X + p2.X);
    }

    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.Y - p2.Y, p1.X - p2.X);
    }

    public static implicit operator (int Row, int Col)(Point p)
    {
        return (p.Row, p.Col);
    }

    public static implicit operator Point((int Row, int Col) p)
    {
        return new Point(p.Row, p.Col);
    }

    public bool Equals(Point other)
    {
        return Col == other.Col && Row == other.Row;
    }

    public override bool Equals(object obj)
    {
        return obj is Point other && Equals(other);
    }

    public static bool operator ==(Point left, Point right) => left.Equals(right);
    public static bool operator !=(Point left, Point right) => !left.Equals(right);

    public override int GetHashCode()
    {
        return HashCode.Combine(Col, Row);
    }

    public void Deconstruct(out int x, out int y)
    {
        x = Col;
        y = Row;
    }
    
    public double DistanceTo(Point other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

public enum Direction
{
    Up = 1,
    Down,
    Left,
    Right
}

[DebuggerDisplay("Start:{Start},End:{End}")]
public struct Slice : IEquatable<Slice>
{
    public Slice()
    {
    }

    public Slice(Point start, Point end)
    {
        this.Start = start;
        this.End = end;
    }

    public Direction Direction => GetDirection();

    private Direction GetDirection()
    {
        if (IsPoint)
        {
            throw new Exception("No direction!");
        }

        if (!IsHorizontal && !IsVertical)
        {
            throw new Exception("No direction!");
        }

        if (IsHorizontal)
        {
            return Start.X < End.X ? Direction.Right : Direction.Left;
        }

        if (IsVertical)
        {
            return Start.Y < End.Y ? Direction.Down : Direction.Up;
        }

        throw new Exception("No direction for freak slice");
    }

    public Point Start;
    public Point End;

    public bool HasPoint(Point point)
    {
        if (IsPoint) return Start == point;

        if (IsHorizontal)
            return Start.X <= point.X && point.X <= End.X
                   || Start.X >= point.X && point.X >= End.X;
        
        if (IsVertical)
            return Start.Y <= point.Y && point.Y <= End.Y
                   || Start.Y >= point.Y && point.Y >= End.Y;

        throw new Exception("No vertical or horizontal");
    }
    
    public Point[] EdgePoints => [Start, End];

    public bool Equals(Slice other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override bool Equals(object obj)
    {
        return obj is Slice other && Equals(other);
    }

    public static bool operator ==(Slice left, Slice right) => left.Equals(right);
    public static bool operator !=(Slice left, Slice right) => !left.Equals(right);

    public static implicit operator (Point Left, Point Right)(Slice p)
    {
        return (p.Start, p.End);
    }

    public static implicit operator Slice((Point Left, Point Right) p)
    {
        return new Slice(p.Left, p.Right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public bool IsPoint => IsHorizontal && IsVertical;

    public bool HasIntersection(Slice other, out Point intersectionPoint)
    {
        if (IsPoint && other.IsPoint)
        {
            intersectionPoint = default;
            return Start == other.Start;
        }

        Slice horizontal, vertical;
        if (IsPoint)
        {
            vertical = other.IsVertical ? other : this;
            horizontal = other.IsHorizontal ? other : this;
        }
        else
        {
            horizontal = IsHorizontal ? this : other;
            vertical = IsVertical ? this : other;
        }

        var verticalX = vertical.Start.X;
        var horizontalY = horizontal.Start.Y;

        intersectionPoint = new Point(horizontalY, verticalX);

        return Math.Min(horizontal.Start.X, horizontal.End.X) <= verticalX && verticalX <=
                                                                           Math.Max(horizontal.Start.X,
                                                                               horizontal.End.X)
                                                                           && Math.Min(vertical.Start.Y,
                                                                               vertical.End.Y) <= horizontalY &&
                                                                           horizontalY <= Math.Max(vertical.Start.Y,
                                                                               vertical.End.Y);
    }

    public bool HasEdgePoint(Point point)
    {
        return point == End || point == Start;
    }

    public bool IsVertical => Start.X == End.X;
    public bool IsHorizontal => Start.Y == End.Y;

    public IEnumerable<Point> GetPoints()
    {
        var from = 0;
        var to = 0;

        if (IsVertical)
        {
            from = Math.Min(Start.Y, End.Y);
            to = Math.Max(Start.Y, End.Y);

            for (var i = from; i <= to; ++i) yield return new Point(i, Start.X);
            yield break;
        }
        else if (IsHorizontal)
        {
            from = Math.Min(Start.X, End.X);
            to = Math.Max(Start.X, End.X);

            for (var i = from; i <= to; ++i) yield return new Point(Start.Y, i);
            yield break;
        }

        throw new Exception("Not horizontal or vertical");
    }

    public void Deconstruct(out Point start, out Point end)
    {
        start = Start;
        end = End;
    }

    public bool IsPointOnRightSide(Point otherPoint)
    {
        if (IsPoint) throw new Exception("No sides on point");

        if (Direction == Direction.Down)
            return otherPoint.X < Start.X;
        if (Direction == Direction.Up)
            return otherPoint.X > Start.X;
        
        if (Direction == Direction.Left)
            return otherPoint.Y < Start.Y;
        if (Direction == Direction.Right)
            return otherPoint.Y > Start.Y;
        
        throw new Exception("Unknown direction");
    }
}