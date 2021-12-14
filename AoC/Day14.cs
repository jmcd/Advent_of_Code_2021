namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day14
{
    [Theory]
    [InlineData("day14_example.txt", 10, 1588)]
    [InlineData("day14.txt", 10, 2602)]
    [InlineData("day14_example.txt", 40, 2188189693529)]
    [InlineData("day14.txt", 40, 2942885922173)]
    public async Task Part1And2(string filename, int numberOfSteps, long expectation)
    {
        var (polymerTemplate, pairInsertions) = await ReadInput(filename);

        var elementCount = new Dictionary<char, long>();
        foreach (var element in polymerTemplate)
        {
            elementCount[element] = elementCount.GetValueOrDefault(element, 0) + 1;
        }

        var elementPairCount = pairInsertions.ToDictionary(kvp => kvp.Key, _ => 0L);
        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            var pair = new Pair(polymerTemplate[i - 1], polymerTemplate[i]);
            elementPairCount[pair] = elementPairCount.GetValueOrDefault(pair, 0) + 1;
        }

        for (var i = 0; i < numberOfSteps; i++)
        {
            foreach (var ((pair, element), pairCountSnapshot) in pairInsertions.Select(pi => (pi, elementPairCount[pi.Key])).ToArray())
            {
                // Update the element count
                elementCount[element] = elementCount.GetValueOrDefault(element, 0) + pairCountSnapshot;

                // Remove the count from the current pair
                elementPairCount[pair] -= pairCountSnapshot;

                // Reassign the count to the two new replacement pairs
                elementPairCount[new Pair(pair.Left, element)] += pairCountSnapshot;
                elementPairCount[new Pair(element, pair.Right)] += pairCountSnapshot;
            }
        }

        var diff = elementCount.Max(kvp => kvp.Value) - elementCount.Min(kvp => kvp.Value);
        Assert.Equal(expectation, diff);
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

    private record struct Pair(char Left, char Right);
}