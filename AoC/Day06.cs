namespace AoC;

using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day06
{
    [Theory]
    [InlineData("day06_example.txt", 18, 26)]
    [InlineData("day06_example.txt", 80, 5934)]
    [InlineData("day06_example.txt", 256, 26984457539)]
    [InlineData("day06.txt", 80, 372984)]
    [InlineData("day06.txt", 256, 1681503251694)]
    public async Task Part1And2(string filename, int numberOfDays, long expectation)
    {
        var fishAges = (await Input.ReadSingleLineAsync(filename)).Split(",").Select(int.Parse).ToList();

        const int maxAge = 8;

        var startIndex = 0;
        var numberOfFishAtAgeIndicatedByArrayIndex = new long[maxAge + 1];

        foreach (var fishAge in fishAges)
        {
            numberOfFishAtAgeIndicatedByArrayIndex[fishAge]++;
        }

        // Instead of decrementing the array, just change what the start (and end) index of the array is

        for (var i = 0; i < numberOfDays; i++)
        {
            var endIndex = (startIndex + numberOfFishAtAgeIndicatedByArrayIndex.Length - 2) % numberOfFishAtAgeIndicatedByArrayIndex.Length;
            numberOfFishAtAgeIndicatedByArrayIndex[endIndex] += numberOfFishAtAgeIndicatedByArrayIndex[startIndex];
            startIndex = (startIndex + 1) % numberOfFishAtAgeIndicatedByArrayIndex.Length;
        }

        Assert.Equal(expectation, numberOfFishAtAgeIndicatedByArrayIndex.Sum());
    }
}