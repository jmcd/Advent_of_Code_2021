namespace AoC;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Day06
{
    private ITestOutputHelper _op;

    public Day06(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("day06_example.txt", 5934)]
    [InlineData("day06.txt", 372984)]
    public async Task Part1(string filename, int expectation)
    {
        var fishes = (await Input.ReadSingleLineAsync(filename)).Split(",").Select(int.Parse).ToList();

        for (var day = 1; day <= 80; day++)
        {
            var nextFishes = new List<int>();
            var numberOfNewFish = 0;
            foreach (var fish in fishes)
            {
                int nextFish;
                if (fish == 0)
                {
                    nextFish = 6;
                    numberOfNewFish++;
                }
                else
                {
                    nextFish = fish - 1;
                }
                nextFishes.Add(nextFish);
            }
            for (var newFishIndex = 0; newFishIndex < numberOfNewFish; newFishIndex++)
            {
                nextFishes.Add(8);
            }
            fishes = nextFishes;
        }

        Assert.Equal(expectation, fishes.Count());
    }

    private void DumpFishes(IEnumerable<int> fishes)
    {
        _op.WriteLine(string.Join(',', fishes));
    }
}