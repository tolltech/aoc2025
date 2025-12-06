using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task06
{
    [Test]
    [TestCase(
        @"123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  ",
        4277556)]
    [TestCase(@"Task06.txt", 5595593539811L)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitLines();
        var numbers = lines.Take(lines.Length - 1).Select(x => x.SplitEmpty().Select(long.Parse).ToArray()).ToArray();
        var ops = lines.Last().SplitEmpty();

        for (var j = 0; j < ops.Length; ++j)
        {
            var sum = ops[j].Contains('+');
            var acc = sum ? 0L : 1L;
            for (var i = 0; i < numbers.Length; ++i)
            {
                if (sum)
                    acc += numbers[i][j];
                else
                    acc *= numbers[i][j];
            }
            
            result += acc;
        }
        
        result.Should().Be(expected);
    }
}