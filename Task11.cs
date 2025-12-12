using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task11
{
    [Test]
    [TestCase(
        @"aaa: you hhh
you: bbb ccc
bbb: ddd eee
ccc: ddd eee fff
ddd: ggg
eee: out
fff: out
ggg: out
hhh: ccc fff iii
iii: out",
        5)]
    [TestCase(@"Task11.txt", 796)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();
        var paths = new Dictionary<string, string[]>();
        foreach (var line in lines)
        {
            var splits = line.SplitEmpty(":", " ");
            var src = splits[0];
            var dst = splits.Skip(1).ToArray();
            paths[src] = dst;
        }

        var cache = new Dictionary<(string, int), long>();
        result = Dfs(paths, "you", 0, cache);
        
        result.Should().Be(expected);
    }
    
    private long Dfs(Dictionary<string, string[]> graph, string device, int level, Dictionary<(string, int), long> cache)
    {
        var current = (device, level);
        if (cache.TryGetValue(current, out var value))
        {
            return value;
        }

        if (device == "out")
        {
            cache[current] = 1;
            return 1;
        }

        var children = graph[device];

        var result = 0L;

        foreach (var child in children)
        {
            result += Dfs(graph, child, level + 1, cache);
        }

        return result;
    }
}