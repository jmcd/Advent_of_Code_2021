namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day03
{
    private static async Task<(int numberWidth, ICollection<int> numbers)> ReadInputAsync(string filename)
    {
        var lines = (await Input.ReadAllLinesAsync(filename)).ToList();
        var lineLength = lines[0].Length;
        var numbers = lines.Select(s => Convert.ToInt32(s, 2)).ToList();
        return (lineLength, numbers);
    }

    [Theory]
    [InlineData("day03_example.txt", 22 * 9)]
    [InlineData("day03.txt", 2663 * 1432)]
    public async Task Part1(string filename, int expectation)
    {
        var (numberWidth, numbers) = await ReadInputAsync(filename);
        var numberCount = numbers.Count;
        var oneCounts = new int[numberWidth];

        foreach (var number in numbers)
        {
            for (var bitIndex = 0; bitIndex < numberWidth; bitIndex++) oneCounts[numberWidth - 1 - bitIndex] += (number >> bitIndex) & 1;
        }

        var gamma = 0;
        for (var bitIndex = 0; bitIndex < oneCounts.Length; bitIndex += 1)
        {
            if (oneCounts[bitIndex] > numberCount / 2)
                gamma |= 1 << (oneCounts.Length - 1 - bitIndex);
        }

        var epsilon = ~gamma & ((1 << numberWidth) - 1);
        Assert.Equal(expectation, gamma * epsilon);
    }

    [Theory]
    [InlineData("day03_example.txt", 23 * 10)]
    [InlineData("day03.txt", 2526 * 1184)]
    public async Task Part2(string filename, int expectation)
    {
        var (numberWidth, numbers) = await ReadInputAsync(filename);
        int Filter(Func<(int zeroCount, int oneCount), bool> predicateOfBitCount) => FilterSubset(numbers, predicateOfBitCount, numberWidth - 1);

        int FilterSubset(ICollection<int> numberSubset, Func<(int oneCount, int zeroCount), bool> predicateOfBitCount, int bitIndex)
        {
            if (numberSubset.Count == 1) return numberSubset.First();

            var oneCount = numberSubset.Count(i => i.GetBit(bitIndex) == 1);
            var filterBit = predicateOfBitCount((numberSubset.Count - oneCount, oneCount)) ? 1 : 0;
            var numberSubSubset = numberSubset.Where(i => i.GetBit(bitIndex) == filterBit).ToList();
            return FilterSubset(numberSubSubset, predicateOfBitCount, bitIndex - 1);
        }

        var oxygenGeneratorRating = Filter(bitCounts => bitCounts.oneCount >= bitCounts.zeroCount);
        var co2ScrubberRating = Filter(bitCounts => bitCounts.oneCount < bitCounts.zeroCount);
        Assert.Equal(expectation, oxygenGeneratorRating * co2ScrubberRating);
    }
}