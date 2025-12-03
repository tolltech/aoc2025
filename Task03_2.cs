using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task03_2
{
    [Test]
    [TestCase(
        @"987654321111111
811111111111119
234234234234278
818181911112111",
        3121910778619)]
    [TestCase(@"Task03.txt", 169019504359949)]
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
        var result = new List<char>();
        var left = 12;
        var maxLeft = -1;
        while (--left >= 0)
        {
            var max = number.Take(number.Length - left).Select((c, i) => (c, i))
                .Where(x => x.i > maxLeft)
                .OrderByDescending(x => x.c)
                .ThenBy(x => x.i).First();
            result.Add(max.c);

            if (maxLeft < max.i) maxLeft = max.i;
        }

        var s = new string(result.ToArray());
        return long.Parse(s);
    }
}