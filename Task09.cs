using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task09
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
        50)]
    [TestCase(@"Task09.txt", 4760959496)]
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

        foreach (var left in points)
        foreach (var right in points)
        {
            if (left == right) continue;

            var square = (Math.Abs(left.X - right.X) + 1L)
                         * (Math.Abs(left.Y - right.Y) + 1);
            
            if (result < square) result = square;
        }
        
        result.Should().Be(expected);
    }
}