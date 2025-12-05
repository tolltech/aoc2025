using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task05
{
    [Test]
    [TestCase(
        @"3-5
10-14
16-20
12-18

1
5
8
11
17
32",
        3)]
    [TestCase(@"Task05.txt", 611)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();

        var ranges = new List<(long start, long end)>();
        foreach (var line in lines)
        {
            if (line.Contains('-'))
            {
                var splits = line.SplitEmpty("-").Select(long.Parse).ToArray();
                ranges.Add((splits[0], splits[1]));
                continue;
            }

            if (long.TryParse(line, out var id))
            {
                foreach (var range in ranges)
                {
                    if (id >= range.start && id <= range.end)
                    {
                        ++result;
                        break;
                    }
                }
            }
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