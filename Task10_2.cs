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

            (a, b) = Normalize(a, b);

            if (b.Length == a[0].Length)
            {
                var xs = Solve(a.Select(x => x.Select(y => (double)y).ToArray()).ToArray(), b)
                    .Select(Convert.ToInt32).ToArray();

                if (xs.Any(x => x < 0)) throw new Exception("Negative value");

                result += xs.Sum();
            }
            else if (b.Length < a[0].Length)
            {
                var max = b.Max();
                for (var i = 0; i <= max; ++i)
                {
                }
            }
        }

        result.Should().Be(expected);
    }

    private (List<int[]> a, int[] b) Normalize(List<int[]> a, int[] b)
    {
        var newA = new List<int[]>();
        var newB = new List<int>();
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

    public double[] Solve(double[][] matrix, int[] freeMembers)
    {
        if (matrix.GetLength(0) != freeMembers.Length)
            throw new ArgumentException();

        const double eps = 1e-9;
        int m = matrix.Length, n = matrix[0].Length;
        var fullMatrix = matrix.Select((row, i) => row.Concat([freeMembers[i]]).ToArray()).ToArray();
        var resRows = Enumerable.Repeat(-1, n).ToArray();
        for (int col = 0, row = 0; row < m && col < n; col++)
        {
            var maxInCol = Enumerable.Range(row, m - row).Select(rowIndex =>
                    (absValue: Math.Abs(fullMatrix[rowIndex][col]), rowIndex))
                .Max();
            if (maxInCol.absValue < eps)
                continue;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var rowWithMaxElement = maxInCol.rowIndex;
            Swap(fullMatrix, row, rowWithMaxElement);
            for (var r = 0; r < m; r++)
                if (r != row)
                {
                    var coefficient = fullMatrix[r][col] / fullMatrix[row][col];
                    for (var c = col; c <= n; c++)
                        fullMatrix[r][c] -= fullMatrix[row][c] * coefficient;
                }

            resRows[col] = row++;
        }

        if (!IsSolution(fullMatrix, eps))
            throw new Exception("No solution");

        // Если остались row с не нулевыми значениями в последнем столбце (свободных членах), то решений нет.
        // Если не осталось row, или все оставшиеся row состоят из нулей, то решение есть и вычисляется так ↓
        return resRows.Select((row, col) => row < 0 ? 0.0 : fullMatrix[row][n] * 1.0 / fullMatrix[row][col]).ToArray();
    }

    private static void Swap<T>(T[] array, int i, int j)
    {
        (array[i], array[j]) = (array[j], array[i]);
    }

    private static bool IsSolution(double[][] matrix, double accuracy)
    {
        foreach (var row in matrix)
        {
            var rowWithoutFreeMember = row.Take(row.Length - 1);
            var freeMember = row.Last();
            if (rowWithoutFreeMember.All(n => Math.Abs(n) < accuracy) && Math.Abs(freeMember) > accuracy)
                return false;
        }

        return true;
    }
}