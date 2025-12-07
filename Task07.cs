using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task07
{
    [Test]
    [TestCase(
        @".......S.......
.......|.......
......|^|......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............  ",
        21)]
    [TestCase(@"Task07.txt", 1662)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();
        var beams = new HashSet<int>();

        beams.Add(lines.First().IndexOf('S'));

        foreach (var line in lines.Skip(1))
        {
            foreach (var beam in beams.ToArray())
            {
                var c = line[beam];
                if (c == '.') continue;
                if (c == '^')
                {
                    result++;
                    beams.Remove(beam);
                    SafeAdd(beams, beam - 1, line);
                    SafeAdd(beams, beam + 1, line);
                }
            }
        }

        result.Should().Be(expected);
    }

    private static void SafeAdd(HashSet<int> beams, int beam, string line)
    {
        if (beam < 0) return;
        if (beam >= line.Length) return;
        beams.Add(beam);
    }
}