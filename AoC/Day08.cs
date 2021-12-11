namespace AoC;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Day08
{
    [Theory]
    [InlineData("day08_example02.txt", 26)]
    [InlineData("day08.txt", 352)]
    public async Task Part1(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var entries = (await Input.ReadAllLinesAsync(filename)).Select(Parse).ToList();

        var defs = new[]
        {
            "abcefg",
            "cf",
            "acdeg",
            "acdfg",
            "bcdf",
            "abdfg",
            "abdefg",
            "acf",
            "abcdefg",
            "abcdfg",
        };

        var targets = new[] { 1, 4, 7, 8 };
        var count = 0;
        foreach (var target in targets)
        {
            var targetDef = defs[target];
            foreach (var (_, output) in entries)
            {
                foreach (var pattern in output)
                {
                    if (pattern.Length != targetDef.Length) { continue; }
                    count++;
                }
            }
        }

        Assert.Equal(expectation, count);
    }

    [Theory]
    [InlineData("day08_example01.txt", 5353)]
    [InlineData("day08_example02.txt", 61229)]
    [InlineData("day08.txt", 936117)]
    public async Task Part2(string filename, int expectation)
    {
        var entries = (await Input.ReadAllLinesAsync(filename)).Select(Parse).ToList();

        var valueSum = 0;
        foreach (var (signal, output) in entries)
        {
            var signalPatterns = signal.Select(pattern => pattern.ToHashSet()).ToArray();
            var outputPatterns = output.Select(pattern => pattern.ToHashSet()).ToArray();

            var numberToSignalPattern = new HashSet<char>?[10];

            foreach (var signalPattern in signalPatterns)
            {
                switch (signalPattern.Count)
                {
                    case 2:
                        numberToSignalPattern[1] = signalPattern;
                        break;
                    case 3:
                        numberToSignalPattern[7] = signalPattern;
                        break;
                    case 4:
                        numberToSignalPattern[4] = signalPattern;
                        break;
                    case 7:
                        numberToSignalPattern[8] = signalPattern;
                        break;
                }
            }

            foreach (var signalPattern in signalPatterns)
            {
                switch (signalPattern.Count)
                {
                    case 6:
                    {
                        if (signalPattern.IsSupersetOf(numberToSignalPattern[4]!))
                        {
                            numberToSignalPattern[9] = signalPattern;
                        }
                        break;
                    }
                    case 5:
                    {
                        if (signalPattern.IsSupersetOf(numberToSignalPattern[4]!.Except(numberToSignalPattern[1]!)))
                        {
                            numberToSignalPattern[5] = signalPattern;
                        }
                        break;
                    }
                }
            }

            foreach (var signalPattern in signalPatterns)
            {
                switch (signalPattern.Count)
                {
                    case 6 when signalPattern.SetEquals(numberToSignalPattern[9]!):
                        continue;
                    case 6 when signalPattern.IsSupersetOf(numberToSignalPattern[5]!):
                        numberToSignalPattern[6] = signalPattern;
                        break;
                    case 6:
                        numberToSignalPattern[0] = signalPattern;
                        break;
                    case 5 when signalPattern.SetEquals(numberToSignalPattern[5]!):
                        continue;
                    case 5 when numberToSignalPattern[9]!.IsSupersetOf(signalPattern):
                        numberToSignalPattern[3] = signalPattern;
                        break;
                    case 5:
                        numberToSignalPattern[2] = signalPattern;
                        break;
                }
            }

            var value = 0;
            for (var i = 0; i < 4; i++)
            {
                var outputPattern = outputPatterns[i];
                for (var number = 0; number < numberToSignalPattern.Length; number++)
                {
                    var signalPattern = numberToSignalPattern[number];
                    if (outputPattern.SetEquals(signalPattern!))
                    {
                        value = value * 10 + number;
                    }
                }
            }

            valueSum += value;
        }

        Assert.Equal(expectation, valueSum);
    }

    private static (string[] signal, string[] output) Parse(string line)
    {
        var sigAndOp = line.Split("|").Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToArray();
        return (signal: sigAndOp[0], output: sigAndOp[1]);
    }
}