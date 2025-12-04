using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task04
{
    [Test]
    [TestCase(
        @"..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.",
        13)]
    [TestCase(@"Task04.txt", 1460L)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines().Select(x => x.ToArray()).ToArray();
        
        for (int i = 0; i < lines.Length; i++)
        for (int j = 0; j < lines[i].Length; j++)
        {
            if (lines[i][j] != '@') continue;
            
            var rolls = Extensions.GetAllNeighbours(lines, (i, j)).Count(x => x.Item == '@');
            if (rolls < 4) 
                ++result;
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