using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task12
{
    [Test]
    [TestCase(
        @"0:
###
##.
##.

1:
###
##.
.##

2:
.##
###
##.

3:
##.
###
##.

4:
###
#..
###

5:
###
.#.
###

4x4: 0 0 0 0 2 0
12x5: 1 0 1 0 2 2
12x5: 1 0 1 0 3 2",
        2)]
    [TestCase(@"Task12.txt", 0)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;
        var presents = new Dictionary<int, Present>();

        var presentsStr = input.SplitEmpty(Environment.NewLine + Environment.NewLine)
            .Where(x => x.Contains("#"))
            .ToArray();
        
        foreach (var str in presentsStr)
        {
            var hashSet = new HashSet<Point>();
            var lines = str.SplitLines();
            var index = int.Parse(lines.First().Trim(':'));

            for (var i = 1; i < lines.Length; i++)
            {
                for (var j = 0; j < lines[i].Length; j++)
                {
                    if (lines[i][j] == '#')
                        hashSet.Add(new Point(i - 1, j));
                }
            }

            var present = new Present
            {
                Points = hashSet,
                Index = index
            };
            presents[present.Index] = present;
        }

        var regions = input.SplitLines()
            .Where(x => x.Contains("x"))
            .Select(line =>
            {
                var splits = line.SplitEmpty(": ", "x", " ").Select(int.Parse).ToArray();
                var region = new Point(splits[0], splits[1]);
                var counts = splits.Select((x, i) => (Index: i - 2, Count: x)).Skip(2).ToArray();
                
                return (Region: region, Counts: counts);
            })
            .ToArray();

        foreach (var region in regions)
        {
            if (CheckRegion(region, presents))
                result++;
        }
        
        result.Should().Be(expected);
    }

    private bool CheckRegion((Point Region, (int Index, int Count)[] Counts) region, Dictionary<int, Present> presents)
    {
        var regionSquare = region.Region.X * region.Region.Y;
        var spaceNeed = region.Counts.Sum(x => presents[x.Index].Points.Count * x.Count);

        if (spaceNeed > regionSquare) return false;

        return true;
    }

    private struct Present
    {
        public int Index { get; set; }
        public HashSet<Point> Points { get; set; }

        private HashSet<Present>? all;

        public HashSet<Present> GetAll() => all ??= Mutate().ToHashSet();
        
        private IEnumerable<Present> Mutate()
        {
            yield return this;
            yield return Turn();
            yield return Turn().Turn();
            yield return Turn().Turn().Turn();
            
            yield return Flip();
            yield return Flip().Turn();
            yield return Flip().Turn().Turn();
            yield return Flip().Turn().Turn().Turn();
        }

        public string Dbg()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 3; ++i)
            {
                for (var j = 0; j < 3; ++j)
                {
                    sb.Append(Points.Contains((i, j)) ? '#' : '.');
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public Present Turn()
        {
            var newSet = new HashSet<Point>();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (!Points.Contains((i,j))) continue;
                    
                    newSet.Add((j, 2 - i));
                }
            }

            return this with { Points = newSet };
        }
        
        public Present Flip()
        {
            var newSet = new HashSet<Point>();
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (!Points.Contains((i,j))) continue;
                    
                    newSet.Add((2 - i, j));
                }
            }

            return this with { Points = newSet };
        }
        
        
    }
}