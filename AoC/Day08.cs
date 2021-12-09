namespace AoC;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Day08
{
    private ITestOutputHelper _op;
    public Day08(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("day08_example.txt", 26)]
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
                    if (pattern.Length != targetDef.Length)
                    {
                        continue;
                    }

                    //_op.WriteLine(pattern);

                    count++;
                }
                // _op.WriteLine("--");
            }
        }

        Assert.Equal(expectation, count);
    }

    [Theory]
    [InlineData("day08_example.txt", 26)]
    //[InlineData("day08.txt", 352)]
    public async Task Part2(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var entries = (await Input.ReadAllLinesAsync(filename)).Select(Parse).ToList();

        var numberToSegments = new[]
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




        //var all = new HashSet<char>("abcdefg").ToImmutableHashSet();

        var orderedNumberSegments = numberToSegments.Select((ns, i) => (number: i, segmentPattern: ns)).OrderBy(tpl => tpl.segmentPattern.Length).ToArray();

        foreach (var (signal, _) in entries)
        {
            var ones = signal.FirstOrDefault(pattern => pattern.Length == 2);
            var fours = signal.FirstOrDefault(pattern => pattern.Length == 4);
            var sevens = signal.FirstOrDefault(pattern => pattern.Length == 3);
            var eights = signal.FirstOrDefault(pattern => pattern.Length == 7);
            _op.WriteLine($"1: {string.Join("", ones ?? "")}");
            _op.WriteLine($"4: {string.Join("", fours ?? "")}");
            _op.WriteLine($"7: {string.Join("", sevens ?? "")}");
            _op.WriteLine($"8: {string.Join("", eights ?? "")}");
            _op.WriteLine("--");

            var oneSegmentsSharedWithFour = new HashSet<char>(numberToSegments[1]).Intersect(numberToSegments[4]);
        }
        /*
        var possibles = "abcdefg".Aggregate(new Dictionary<char, HashSet<char>>(), (dictionary, c) =>
        {
            dictionary[c] = new HashSet<char>("abcdefg");
            return dictionary;
        });

        //var possible = new Dictionary<char, ISet<char>>();
        foreach (var scrambledSegmentPattern in signal)
        {
            foreach (var (number, segmentPattern) in orderedNumberSegments)
            {
                if (segmentPattern.Length != scrambledSegmentPattern.Length)
                {
                    continue;
                }

                foreach (char c in scrambledSegmentPattern)
                {
                    possibles[c].IntersectWith(segmentPattern);
                }
            }
        }



        foreach (var kvp in possibles)
        {
            _op.WriteLine($"{kvp.Key} -> {string.Join("", kvp.Value)}");
        }
        _op.WriteLine("--");
    }
*/

/*
        foreach (var (signal, _) in entries)
        {
            foreach (var scrambledSegmentPattern in signal)
            {
                for (int number = 0; number < numberToSegments.Length; number++)
                {
                    var numberSegments = numberToSegments[number];

                    if (numberSegments.Length != scrambledSegmentPattern.Length)
                    {
                        continue;
                    }

                    foreach (var c in scrambledSegmentPattern)
                    {
                        // c->
                    }
                }


                foreach (var scrambledSegment in scrambledSegmentPattern)
                {
                    for (var number = 0; number < numberToSegments.Length; number++)
                    {
                        var segments = numberToSegments[number];

                        if (scrambledSegmentPattern.Length != segments.Length)
                        {
                            continue;
                        }

                        var hashSet = possibles[scrambledSegment].ToArray();
                        possibles[scrambledSegment].IntersectWith(segments);
                        _op.WriteLine($"{scrambledSegment}: {string.Join("", hashSet)} + {segments} == {string.Join("", possibles[scrambledSegment])}");
                    }
                }
            }
        }


        foreach (var kvp in possibles)
        {
            //Assert.Single(kvp.Value);
            _op.WriteLine($"{kvp.Key}: {string.Join("", kvp.Value)}");
        }
        */

            //Assert.Equal(1, expectation, 1);
        }

        // [Theory]
        // [InlineData("day08_example.txt", 61229)]
        // //[InlineData("day08.txt", 61229)]
        // public async Task Part2(string filename, int expectation)
        // {
        //     var a = 0b000001;
        //     var b = 0b000010;
        //     var c = 0b000100;
        //     var d = 0b001000;
        //     var e = 0b001000;
        //     var f = 0b010000;
        //     var g = 0b100000;
        //
        //     var ix = new[] { a, b, c, d, e, f, g };
        //
        //     Debug.WriteLine(filename, expectation);
        //
        //     // var entries = (await Input.ReadAllLinesAsync(filename)).Select(line =>
        //     // {
        //     //     var sigAndOp = line.Split("|").Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToArray();
        //     //     return (signal: sigAndOp[0], output: sigAndOp[1]);
        //     // }).ToList();
        //     //
        //     //
        //     var entries = (await Input.ReadAllLinesAsync(filename)).Select(line =>
        //     {
        //         var sigAndOp = line.Split("|")
        //             .Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries)
        //                 .Select(patternString => patternString.Select(c1 => ix[c1 - 'a']).ToArray()).ToArray()).ToArray();
        //         return (signal: sigAndOp[0], output: sigAndOp[1]);
        //     }).ToList();
        //
        //     var numberToSegment = new[]
        //     {
        //         a | b | c | e | f | g,
        //         c | f,
        //         a | c | d | e | g,
        //         a | c | d | f | g,
        //         b | c | d | f,
        //         a | b | d | f | g,
        //         a | b | d | e | f | g,
        //         a | c | f,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | f | g,
        //     };
        //
        //     var numberToSegmentCount = new[]
        //     {
        //         6, 2, 5, 5, 4, 5, 6, 3, 7, 6,
        //     };
        //
        //     var segmentPossibles = new[]
        //     {
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //         a | b | c | d | e | f | g,
        //     };
        //
        //     // var targets = new[] { 1, 4, 7, 8 };
        //     // var count = 0;
        //     // foreach (var target in targets)
        //     // {
        //     //     v
        //     // }
        //
        //     foreach ((var signal, var output) in entries)
        //     {
        //         foreach (var pattern in signal)
        //         {
        //             for (var number = 0; number < numberToSegment.Length; number++)
        //             {
        //                 var numberSegmentCount = numberToSegmentCount[number];
        //
        //                 if (pattern.Length != numberSegmentCount) { continue;}
        //
        //
        //             }
        //         }
        //     }
        //
        //     Assert.Equal(expectation, count);
        // }

        private static (string[] signal, string[] output) Parse(string line)
        {
            var sigAndOp = line.Split("|").Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries)).ToArray();
            return (signal: sigAndOp[0], output: sigAndOp[1]);
        }
    }