using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task01
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
        3)]
    [TestCase(@"Task01.txt", 1180)]
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

            current = Op(current, delta);

            if (current == 0) ++result;
        }
        
        result.Should().Be(expected);
    }

    private static int Op(int current, int delta)
    {
        var normDelta = Math.Abs(delta) % 100;
        if (delta < 0)
        {
            delta = 100 - normDelta;
        }
        return (current + delta) % 100;
    }
}