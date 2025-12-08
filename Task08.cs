using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task08
{
    [Test]
    [TestCase(
        @"162,817,812
57,618,57
906,360,560
592,479,940
352,342,300
466,668,158
542,29,236
431,825,988
739,650,466
52,470,668
216,146,977
819,987,18
117,168,530
805,96,715
346,949,466
970,615,88
941,993,340
862,61,35
984,92,344
425,690,689",
        10, 40)]
    [TestCase(@"Task08.txt", 1000, 175500)]
    public void Task(string input, int steps, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var points = input.SplitLines().Select(x =>
            {
                var splits = x.SplitEmpty(",").Select(int.Parse).ToArray();
                return new Point3D(splits[0], splits[1], splits[2]);
            })
            .ToArray();

        var totaCircuits = new List<HashSet<Point3D>>();

        var pairs = new List<(Point3D, Point3D)>();
        for (var i = 0; i < points.Length; i++)
        for (var j = i + 1; j < points.Length; j++)
        {
            pairs.Add((points[i], points[j]));
        }

        pairs = pairs.Distinct()
            .OrderBy(x => x.Item1.DistanceTo(x.Item2))
            .ToList();

        for (var i = 0; i < steps; ++i)
        {
            var pair =  pairs[i];
            var circuits = totaCircuits.Where(x => x.Contains(pair.Item1) || x.Contains(pair.Item2)).ToArray();
            var circuit = GetCircuit(circuits, out var wasUnion);
            
            if (circuit == null)
                totaCircuits.Add([pair.Item1, pair.Item2]);
            else
            {
                if (wasUnion)
                {
                    foreach (var c in circuits)
                    {
                        totaCircuits.Remove(c);
                    }
                    totaCircuits.Add(circuit);
                }
                else
                {
                    circuit.Add(pair.Item1);
                    circuit.Add(pair.Item2);   
                }
            }
        }

        var largest = totaCircuits.Select(x => (long)x.Count).OrderByDescending(x => x).Take(3).ToArray();

        result = largest[0] * largest[1] * largest[2];
        
        result.Should().Be(expected);
    }

    private HashSet<Point3D> GetCircuit(HashSet<Point3D>[] circuits, out bool wasUnion)
    {
        wasUnion = false;
        if (circuits.Length == 0) return null;
        if (circuits.Length == 1) return circuits.Single();
        
        wasUnion = true;
        return circuits.SelectMany(x => x).ToHashSet();
    }

    private static void SafeAdd(HashSet<int> beams, int beam, string line)
    {
        if (beam < 0) return;
        if (beam >= line.Length) return;
        beams.Add(beam);
    }
}