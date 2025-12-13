using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task10_2
{
    [Test]
    [TestCase(
        @"[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}",
        33)]
    [TestCase(@"Task10.txt", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;
        var maxDelta = 0;
        var maxB = 0;

        foreach (var line in input.SplitLines())
        {
            var splits = line.SplitEmpty(" ");

            var buttons = splits.Skip(1).Take(splits.Length - 2)
                .Select(x => x.Trim('(', ')').SplitEmpty(",").Select(int.Parse).ToArray())
                .ToList();

            var b = splits.Last().Trim('{', '}').SplitEmpty(",").Select(int.Parse).ToArray();

            var delta = buttons.Count - b.Length;
            if (maxDelta < delta) maxDelta = delta;
            if (maxB < b.Max()) maxB = b.Max();
            

            var a = b.Select(_ => Enumerable.Repeat(0, buttons.Count).ToArray()).ToList();
            for (var i = 0; i < a.Count; i++)
            for (var j = 0; j < a[i].Length; ++j)
            {
                if (buttons[j].Contains(i))
                    a[i][j] = 1;
                else
                    a[i][j] = 0;
            }

            if (b.Length >= buttons.Count)
            {
                result = SolveSquareMatrix(buttons, a, b).Sum();
            }
        }

        result.Should().Be(expected);
    }

    private IEnumerable<long> SolveSquareMatrix(List<int[]> x, List<int[]> a, int[] b)
    {
        for (var i = 0; i < x.Count; ++i)
        {
            yield return SolveMatrix(a, b, i);
        }
    }

    private long SolveMatrix(List<int[]> a, int[] b, int i)
    {
        return 0;
    }
}