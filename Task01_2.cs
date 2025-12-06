using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task01_2
{
    [Test]
    [TestCase(
        @"L68
L30
R48
L5
R60
L55
L1
L99
R14
L82",
        6)]
    [TestCase(@"Task01.txt", 6892)]
    public void Task(string input, int expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0;

        var lines = input.SplitLines();

        var current = 50;
        foreach (var line in lines)
        {
            var delta = int.Parse(new string(line.Where(char.IsDigit).ToArray()));
            if (line.Contains('L'))
            {
                delta = -delta;
            }

            current = Op(current, delta, out var zeroCount);

            result += zeroCount;
        }

        result.Should().Be(expected);
    }

    private static int Op(int current, int delta, out int zeroCount)
    {
        zeroCount = Math.Abs(delta) / 100;

        var normDelta = Math.Abs(delta) % 100;
        var normSignDelta = Math.Sign(delta) * normDelta;

        if (current != 0 && (current + normSignDelta >= 100 || current + normSignDelta <= 0))
        {
            ++zeroCount;
        }

        if (delta < 0)
        {
            delta = 100 - normDelta;
        }

        return (current + delta) % 100;
    }
}