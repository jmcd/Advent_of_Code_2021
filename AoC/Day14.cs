namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class Day14
{
    [Theory]
    [InlineData("day14_example.txt", 1588)]
    [InlineData("day14.txt", 2602)]
    public async Task Part1(string filename, int expectation)
    {
        var (polymerTemplate, pairInsertions) = await ReadInput(filename);

        for (var i = 0; i < 10; i++)
        {
            var pairs = Pairs(polymerTemplate).ToArray();

            var next = new StringBuilder().Append(pairs[0].Left);
            foreach (var pair in pairs)
            {
                if (pairInsertions.TryGetValue(pair, out var insertion))
                {
                    next.Append(insertion);
                }
                next.Append(pair.Right);
            }
            polymerTemplate = next.ToString();
        }

        var (min, max) = GetMinMaxElementCount(polymerTemplate);

        Assert.Equal(expectation, max.Count - min.Count);
    }

    private static ((char Element, int Count) Min, (char Element, int Count) Max) GetMinMaxElementCount(string polymerTemplate)
    {
        if (polymerTemplate.Length == 0) { throw new ArgumentException($"{nameof(polymerTemplate)} is zero length", nameof(polymerTemplate)); }

        var counts = polymerTemplate.ToCountDictionary();

        KeyValuePair<char, int>? min = null;
        KeyValuePair<char, int>? max = null;
        foreach (var kvp in counts)
        {
            if (min is null || kvp.Value < min.Value.Value) { min = kvp; }
            if (max is null || kvp.Value > max.Value.Value) { max = kvp; }
        }

        return ((min!.Value.Key, min!.Value.Value), (max!.Value.Key, max!.Value.Value));
    }

    private static async Task<(string polymerTemplate, Dictionary<Pair, char> pairInsertions)> ReadInput(string filename)
    {
        var lines = await Input.ReadAllLinesAsync(filename);

        var polymerTemplate = default(string?);
        var pairInsertions = new Dictionary<Pair, char>();

        var haveReadBlankLine = false;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) { haveReadBlankLine = true; }
            else if (!haveReadBlankLine) { polymerTemplate = line; }
            else { pairInsertions[new(line[0], line[1])] = line[^1]; }
        }

        if (polymerTemplate is null) { throw new Exception("Did not find polymer template"); }

        return (polymerTemplate, pairInsertions);
    }

    private IEnumerable<Pair> Pairs(string polymerTemplate)
    {
        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            yield return new Pair(polymerTemplate[i - 1], polymerTemplate[i]);
        }
    }

    private record struct Pair(char Left, char Right);
}