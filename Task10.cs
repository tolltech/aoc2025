using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task10
{
    [Test]
    [TestCase(
        @"[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}",
        7)]
    [TestCase(@"Task10.txt", 428)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        foreach (var line in input.SplitLines())
        {
            var splits = line.SplitEmpty(" ");
            var machine = new Machine(splits[0]);

            var buttons = splits.Skip(1).Take(splits.Length - 2).Select(x => ParseButton(x, splits[0].Length - 2))
                .ToArray();

            var dist = Dijkstra(machine, buttons);
            result += dist[machine with { Value = machine.Goal }];
        }

        result.Should().Be(expected);
    }
    
    private Dictionary<Machine, long> Dijkstra(Machine start, int[] buttons)
    {
        var dist = new Dictionary<Machine, long>();
        var marked = new HashSet<Machine>();

        var pq = new PriorityQueue<Machine, long>();

        dist[start] = 0;

        pq.Enqueue(start, 0);

        while (pq.Count > 0)
        {
            var v = pq.Dequeue();
            if (!marked.Add(v)) continue;

            var nextVs = GetNextV(v, buttons);

            foreach (var nextV in nextVs)
            {
                if (!dist.ContainsKey(nextV) || dist[nextV] > dist[v] + 1)
                {
                    dist[nextV] = dist[v] + 1;
                    pq.Enqueue(nextV, dist[nextV]);
                }
            }
        }

        return dist;
    }

    private IEnumerable<Machine> GetNextV(Machine machine, int[] buttons)
    {
        return buttons.Select(button => machine.Push(button));
    }

    private int? Dfs(Machine machine, Dictionary<int, int> buttons, int pushCount, HashSet<int> states, int minResult,
        Dictionary<(int, int), int> cache)
    {
        if (cache.TryGetValue((machine.Value, pushCount), out var val)) return val;
        if (machine.IsReady)
        {
            cache[(machine.Value, pushCount)] = pushCount; 
            return pushCount;
        }
        
        if (states.Contains(machine.Value)) return null;
        if (pushCount > minResult) return null;

        var result = int.MaxValue;
        foreach (var button in buttons.Keys)
        {
            states.Add(machine.Value);
            var dfs = Dfs(machine.Push(button), buttons, pushCount + 1, states, result, cache);
            if (dfs.HasValue && result > dfs.Value) result = dfs.Value;

            states.Remove(machine.Value);
        }

        return result;
    }

    private static int ParseButton(string src, int machineLength)
    {
        var digits = src.Trim('(', ')').SplitEmpty(",").Select(int.Parse).ToArray();
        var result = new string('0', machineLength).ToCharArray();
        foreach (var digit in digits)
        {
            result[digit] = '1';
        }

        var bin = new string(result);
        return bin.ToIntFromBin();
    }

    private static int ParseMachine(string input)
    {
        return input.Trim('[', ']').Replace(".", "0").Replace("#", "1").ToIntFromBin();
    }

    [DebuggerDisplay("{Dbg}")]
    private record struct Machine(string input)
    {
        public int Goal = ParseMachine(input);
        public int Value;
        public int GoalMask = new string(Enumerable.Repeat('1', input.Length - 2).ToArray()).ToIntFromBin();

        public Machine Push(int button)
        {
            return this with { Value = Value ^ button };
        }

        public bool IsReady => (Value & GoalMask) == (Goal & GoalMask);

        public string Dbg => Value.ToString("B") + "/" + Goal.ToString("B");
    }
}