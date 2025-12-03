using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task03
{
    [Test]
    [TestCase(
        @"987654321111111
811111111111119
234234234234278
818181911112111",
        357)]
    [TestCase(@"Task03.txt", 17087L)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();

        foreach (var line in lines)
        {
            result += Solve(line);
        }

        result.Should().Be(expected);
    }

    private long Solve(string number)
    {
        var max = number.Select((c, i) => (c, i)).OrderByDescending(x => x.c).First();

        if (max.i == number.Length - 1)
        {
            max = number.Take(number.Length - 1).Select((c, i) => (c, i)).OrderByDescending(x => x.c).First();
        }
        
        var max2 = number.Skip(max.i + 1).OrderByDescending(c => c).First().ToString();
        return long.Parse(max.c + max2);
    }
}