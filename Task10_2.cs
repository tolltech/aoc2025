using System.Diagnostics;
using System.Text;
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
        var maxB = 0L;

        foreach (var line in input.SplitLines())
        {
            var splits = line.SplitEmpty(" ");

            var buttons = splits.Skip(1).Take(splits.Length - 2)
                .Select(x => x.Trim('(', ')').SplitEmpty(",").Select(long.Parse).ToArray())
                .ToList();

            var b = splits.Last().Trim('{', '}').SplitEmpty(",").Select(long.Parse).ToArray();

            var delta = buttons.Count - b.Length;
            if (maxDelta < delta) maxDelta = delta;
            //if (maxB < b.Max()) maxB = b.Max();

            var a = new Matrix(b.Select(_ => Enumerable.Repeat(0L, buttons.Count).ToArray()).ToList());
            for (var i = 0; i < a.Count; i++)
            for (var j = 0; j < a[i].Length; ++j)
            {
                if (buttons[j].Contains(i))
                    a[i][j] = 1;
                else
                    a[i][j] = 0;
            }

            Triangle(a, b);

            var m = a.SelectMany(x => x).Concat(b).Max();
            if (maxB < m) maxB = m;

            result = maxB;
        }

        result.Should().Be(expected);
    }

    private long K(List<long[]> a, long[] b)
    {
        var maxs = Enumerable.Repeat(0L, a[0].Length).ToArray();
        for (var j = 0; j < a[0].Length; ++j)
        {
            var max = 1L;
            for (var i = 0; i < a.Count; ++i)
            {
                if (a[i][j] == 0) continue;
                if (max < b[i])
                {
                    max = b[i];
                }
            }

            maxs[j] = max;
        }

        return maxs.Aggregate(1L, (d, d1) => d * d1);
    }

    private (Matrix a, long[] b) Normalize(Matrix a, long[] b)
    {
        var newA = new Matrix();
        var newB = new List<long>();
        var cache = new HashSet<string>();

        for (var i = 0; i < a.Count; i++)
        {
            var line = a[i].JoinToString(",");
            if (!cache.Add(line))
            {
                continue;
            }

            newA.Add(a[i]);
            newB.Add(b[i]);
        }

        return (newA, newB.ToArray());
    }

    private static void Triangle(Matrix a, long[] b)
    {
        for (var i = 0; i < a[0].Length; ++i)
        {
            if (i >= a.Count) break;

            if (a[i][i] == 0)
            {
                var suitableRow = a.Select((row, index) => (row, index)).Skip(i + 1).FirstOrDefault(x => x.row[i] != 0);
                if (suitableRow.row == null) continue;

                Swap(b, i, suitableRow.index);
                Swap(a, i, suitableRow.index);
            }

            Zero(a, b, i);
        }
    }

    private static void Zero(Matrix a, long[] b, int i)
    {
        for (var r = i + 1; r < a.Count; ++r)
        {
            var baseTop = a[i][i];
            var baseBottom = a[r][i];
            
            if (baseBottom == 0) continue;
            
            var newTopRow = a[i].Select(x => x * baseBottom).ToArray();
            var newBottomRow = a[r].Select(x => x * baseTop).ToArray();
            var newRow = newBottomRow.Select((x, index) => x - newTopRow[index]).ToArray();

            b[r] = b[r] * baseTop - b[i] * baseBottom;
            a[r] = newRow;
        }
    }

    [TestCaseSource(nameof(Triangles))]
    public string TriangleTest(Matrix a, long[] b)
    {
        Triangle(a, b);

        return a.Dbg.SplitLines().Select((x, i) => $"{x} {b[i]}").JoinToString(Environment.NewLine);
    }

    private static void Swap<T>(IList<T> array, int i, int j)
    {
        (array[j], array[i]) = (array[i], array[j]);
    }

    public static IEnumerable<TestCaseData> Triangles()
    {
        yield return new TestCaseData(new Matrix
            {
                new long[] { 2, 4, 1 },
                new long[] { 3, 0, 6 },
                new long[] { 9, 1, 2 },
            }, new long[]
            {
                7, 8, 3
            })
            .Returns(@"2 4 1 7
0 -12 9 -5
0 0 366 514");
        yield return new TestCaseData(new Matrix
            {
                new long[] { 2, 4, 1 },
                new long[] { 0, 3, 6 },
                new long[] { 9, 1, 2 },
            }, new long[]
            {
                7, 8, 3
            })
            .Returns(@"2 4 1 7
0 3 6 8
0 0 189 101");
        yield return new TestCaseData(new Matrix
            {
                new long[] { 0, 3, 6 },
                new long[] { 2, 4, 1 },
                new long[] { 9, 1, 2 },
            }, new long[]
            {
                8, 7, 3
            })
            .Returns(@"2 4 1 7
0 3 6 8
0 0 189 101");
        
        yield return new TestCaseData(new Matrix
            {
                new long[] { 0, 3, 6 },
                new long[] { 2, 4, 1 },
                new long[] { 2, 4, 1 },
            }, new long[]
            {
                3, 7, 7
            })
            .Returns(@"2 4 1 7
0 3 6 3
0 0 0 0");
    }

    [DebuggerDisplay("{Dbg}")]
    public class Matrix : List<long[]>
    {
        public Matrix()
        {
            
        }

        public Matrix(IEnumerable<long[]> collection) : base(collection)
        {
            
        }
        
        public string Dbg {
            get
            {
                var sb = new StringBuilder();
                foreach (var row in this)
                {
                    sb.AppendLine(row.JoinToString(" "));
                }
                return  sb.ToString();
            }
        }


    }
}