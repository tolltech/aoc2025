using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task07_2
{
    [Test]
    [TestCase(
        @".......S.......
...............
.......^.......
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
        40)]
    [TestCase(
        @".......S.......
          .......^.......
          ...............
          ......^.^......
          ...............
          .......^.^.....
          ...............
",
        7)]
    [TestCase(@"Task07.txt", 40941112789504)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines().Select(x => x.Trim()).ToArray();
        var beam = lines.First().IndexOf('S');

        var cache = new Dictionary<(int, int), long>();
        result = Dfs(lines, 1, beam, cache);

        //var s = Dbg(lines, cache);

        result.Should().Be(expected);
    }

    private string Dbg(string[] lines, Dictionary<(int, int), long> cache)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            for (var j = 0; j < line.Length; j++)
            {
                var c = line[j];
                if (cache.TryGetValue((i, j), out var value))
                {
                    sb.Append(value % 10);
                }
                else
                {
                    sb.Append(c);
                }
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private long Dfs(string[] lines, int lineIndex, int beam, Dictionary<(int, int), long> cache)
    {
        var current = (lineIndex, beam);
        if (cache.TryGetValue(current, out var value))
        {
            return value;
        }

        if (lineIndex >= lines.Length)
        {
            cache[current] = 1;
            return 1;
        }

        var line = lines[lineIndex];
        var c = line[beam];
        if (c == '.')
        {
            var result = Dfs(lines, lineIndex + 1, beam, cache);
            cache[current] = result;
            return result;
        }

        if (c == '^')
        {
            var result = Dfs(lines, lineIndex + 1, beam - 1, cache)
                         +
                         Dfs(lines, lineIndex + 1, beam + 1, cache);
            cache[current] = result;
            return result;
        }

        throw new ArgumentException();
    }
}