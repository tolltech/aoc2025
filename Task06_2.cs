using System.Numerics;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task06_2
{
    [Test]
    [TestCase(
        @"123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  ",
        3263827)]
    [TestCase(@"Task06.txt", 10153315705125)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = new BigInteger();

        var lines = input.SplitLines();
        var chars = lines.Take(lines.Length - 1).Select(x => x.Select(c => c.ToString()).ToArray()).ToArray();
        var ops = lines.Last().Select(c => c.ToString()).ToArray();

        var sum = false;
        BigInteger acc = 1L;
        for (var j = 0; j < ops.Length; ++j)
        {
            if (ops[j] == "+")
            {
                acc = 0;
                if (!sum)
                {
                    sum = true;
                }
            }
            else if (ops[j] == "*")
            {
                acc = 1;
                if (sum)
                {
                    sum = false;
                }
            }
            
            

            var number = new StringBuilder();
            for (var i = 0; i < chars.Length; ++i)
            {
                number.Append(chars[i][j]);
            }


            var s = number.ToString();
            if (s.All(c => c == ' '))
            {
                result += acc;
                continue;
            }
            if (sum)
                acc += int.Parse(s);
            else
                acc *= int.Parse(s);
        }

        result += acc;

        result.Should().Be(expected);
    }
}