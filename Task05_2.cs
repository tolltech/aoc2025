using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task05_2
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
        14)]
    [TestCase(@"Task05.txt", 345995423801866L)]
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
            }
        }

        var resultRanges = new HashSet<(long start, long end)>();
        while (true)
        {
            if (ranges.Count == 1) break;

            var first = ranges[0];
            if (ranges.Skip(1).All(x => Solve(first, x) == null))
            {
                resultRanges.Add(first);
                ranges = ranges.Skip(1).ToList();
                continue;
            }
            
            var newRanges = new HashSet<(long start, long end)>();

            foreach (var range in ranges.Skip(1))
            {
                var intersection = Solve(first, range);
                newRanges.Add(intersection ?? range);
            }
            
            ranges = newRanges.ToList();
        }

        result = resultRanges.Union(ranges).Sum(x => x.end - x.start + 1);

        result.Should().Be(expected);
    }

    [TestCaseSource(nameof(TestCases))]
    public (long start, long end)? Solve((long start, long end) left, (long start, long end) right)
    {
        if (left.start > right.start) (left, right) = (right, left);

        if (right.start > left.end) return null;

        return (left.start, Math.Max(right.end, left.end));
    }

    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData((1L, 1L), (1L, 3L)).Returns((1L, 3L));
        yield return new TestCaseData((1L, 3L), (1L, 1L)).Returns((1L, 3L));
        yield return new TestCaseData((1L, 1L), (2L, 3L)).Returns(null);
        yield return new TestCaseData((1L, 4L), (2L, 4L)).Returns((1L, 4L));
        yield return new TestCaseData((2L, 4L), (1L, 4L)).Returns((1L, 4L));
        yield return new TestCaseData((2L, 5L), (3L, 4L)).Returns((2L, 5L));
    }
}