using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task11_2
{
    [Test]
    [TestCase(
        @"svr: aaa bbb
aaa: fft
fft: ccc
bbb: tty
tty: ccc
ccc: ddd eee
ddd: hub
hub: fff
eee: dac
dac: fff
fff: ggg hhh
ggg: out
hhh: out",
        2)]
    [TestCase(@"Task11.txt", 294053029111296)]
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

        result = 1;

        var pairs = new[]
        {
            ("svr", "fft"),
            ("fft", "dac"),
            ("dac", "out"),
        };
        foreach (var pair in pairs)
            result *= Dfs(paths, pair.Item1, pair.Item2, 0, cache);

        result.Should().Be(expected);
    }

    private long Dfs(Dictionary<string, string[]> graph, string device, string end, int level,
        Dictionary<(string, int), long> cache)
    {
        var current = (device, level);
        if (cache.TryGetValue(current, out var value))
        {
            return value;
        }

        if (device == end)
        {
            return cache[current] = 1;
        }

        var children = graph.SafeGet(device) ?? [];

        var result = 0L;

        foreach (var child in children)
        {
            result += Dfs(graph, child, end, level + 1, cache);
        }

        return cache[current] = result;
    }
}