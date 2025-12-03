using FluentAssertions;
using NUnit.Framework;

namespace AoC_2025;

[TestFixture]
public class Task02
{
    [Test]
    [TestCase(
        @"11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124",
        1227775554)]
    [TestCase(@"Task02.txt", 40214376723)]
    public void Task(string input, long expected)
    {
        input = File.Exists(input) ? File.ReadAllText(input) : input;

        var result = 0L;

        var lines = input.SplitEmpty(",");
        var ranges = lines.Select(x => x.SplitEmpty("-"))
            .Select(x => (From: long.Parse(x[0]), To: long.Parse(x[1])))
            .ToArray();

        foreach (var range in ranges)
            for (var number = range.From; number <= range.To; ++number)
            {
                if (Invalid(number))
                    result += number;
            }

        result.Should().Be(expected);
    }

    private bool Invalid(long number)
    {
        var numberStr = number.ToString();
        var sub = numberStr.Substring(0, numberStr.Length / 2);

        return sub + sub == numberStr;
    }
}