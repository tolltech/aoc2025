using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task09_2
{
    [Test]
    [TestCase(
        @"7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3",
        24)]
    [TestCase(
        @"7,1
11,1
11,7
9,7
9,5
5,5
5,3
7,3",
        21)]
    [TestCase(@"Task09.txt", 1343576598)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var points = input.SplitLines().Select(x =>
            {
                var splits = x.SplitEmpty(",").Select(int.Parse).ToArray();
                return new Point(splits[1], splits[0]);
            })
            .ToArray();

        var xPoints = points.GroupBy(x => x.X).ToDictionary(x => x.Key, x => x.ToArray());
        var yPoints = points.GroupBy(x => x.Y).ToDictionary(x => x.Key, x => x.ToArray());

        var startPoint = points.OrderBy(x => x.Y).ThenBy(x => x.X).First();
        var currentPoint = startPoint;
        var visitedPoints = new HashSet<Point>();

        var slices = new List<Slice>();

        while (true)
        {
            var nextPoint = xPoints[currentPoint.X]
                .SingleOrDefault(x => x != currentPoint && !visitedPoints.Contains(x));
            if (nextPoint == default)
            {
                nextPoint = yPoints[currentPoint.Y]
                    .Single(x => x != currentPoint && (!visitedPoints.Contains(x) || x == startPoint));
            }

            var slice = new Slice(currentPoint, nextPoint);
            visitedPoints.Add(currentPoint);
            currentPoint = nextPoint;

            if (slice.IsPoint)
            {
                throw new Exception("Slice is point");
            }

            slices.Add(slice);

            if (currentPoint == startPoint) break;
        }

        var horizontalSlices = slices.Where(x => x.IsHorizontal).ToArray();
        var verticalSlices = slices.Where(x => x.IsVertical).ToArray();

        var xSlices = verticalSlices.GroupBy(x => x.Start.X).ToDictionary(x => x.Key, x => x.Single());
        var ySlices = horizontalSlices.GroupBy(x => x.Start.Y).ToDictionary(x => x.Key, x => x.Single());

        foreach (var left in points)
        foreach (var right in points)
        {
            if (left == right) continue;

            var borders = new[]
            {
                new Slice(left, new Point(right.Y, left.X)),
                new Slice(left, new Point(left.Y, right.X)),
                new Slice(right, new Point(left.Y, right.X)),
                new Slice(right, new Point(right.Y, left.X)),
            };

            var isOut = false;

            foreach (var border in borders)
            {
                if (border.IsPoint) continue;

                var suitableSlices = border.IsVertical ? horizontalSlices : verticalSlices;

                var crossSlices = suitableSlices.Where(x => x.HasIntersection(border, out _)).ToArray();

                if (crossSlices.Any(x =>
                        x.HasIntersection(border, out var point) && !x.HasEdgePoint(point) &&
                        !border.HasEdgePoint(point)))
                {
                    isOut = true;
                    break;
                }

                var connectedSlice = border.IsVertical ? xSlices[border.Start.X] : ySlices[border.Start.Y];
                if (connectedSlice == default) throw new Exception("No connectedSlice");

                var sharedPoints = connectedSlice.EdgePoints.Concat(border.EdgePoints).GroupBy(x => x)
                    .Where(x => x.Count() >= 2)
                    .ToArray();

                if (sharedPoints.Length >= 2) continue;
                var sharedPoint = sharedPoints.Single().Key;

                var otherPoint = border.Start == sharedPoint ? border.End : border.Start;
                if (connectedSlice.HasPoint(otherPoint))
                {
                    continue;
                }

                var closestPoint = connectedSlice.EdgePoints.OrderBy(x => x.DistanceTo(otherPoint)).First();
                var directedSlice = border.IsHorizontal ? xSlices[closestPoint.X] : ySlices[closestPoint.Y];

                if (directedSlice.IsPointOnRightSide(otherPoint))
                {
                    isOut = true;
                    break;
                }
            }

            if (isOut)
                continue;

            var square = (Math.Abs(left.X - right.X) + 1L)
                         * (Math.Abs(left.Y - right.Y) + 1);

            if (result < square) result = square;
        }

        result.Should().Be(expected);
    }

    private bool CommonPoints(Slice board, Slice slice)
    {
        var points = new[] { board.Start, board.End, slice.Start, slice.End };
        return points.Distinct().Count() < 4;
    }

    [TestCaseSource(nameof(TestData))]
    public bool Intersects(Slice left, Slice right)
    {
        var (x1, y1) = left.Start;
        var (x2, y2) = left.End;
        var (x3, y3) = right.Start;
        var (x4, y4) = right.End;

        var d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (d == 0) return false;

        var t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / (double)d;
        var u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / (double)d;

        //x = x1 + t * (x2 - x1)
        //y = y1 + t * (y2 - y1)
        //(x, y)

        return t is >= 0 and <= 1
               && u is >= 0 and <= 1;
    }

    private static IEnumerable<TestCaseData> TestData()
    {
        var truePoints = new[]
        {
            new Slice[] { ((0, 0), (1, 1)), ((1, 1), (2, 1)) }
        };

        var falsePoints = new[]
        {
            new Slice[] { ((0, 0), (1, 1)), ((2, 2), (3, 3)) }
        };

        return truePoints.Select(x =>
                new TestCaseData(x[0], x[1]).Returns(true)
            )
            .Concat(
                falsePoints.Select(x => new TestCaseData(x[0], x[1]).Returns(false))
            );
    }
}