namespace AoC;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day11
{
    [Theory]
    [InlineData("day11_example.txt", 1656)]
    [InlineData("day11.txt", 1713)]
    public async void Part1(string filename, int expectation)
    {
        var energyLevels = await ReadInputEnergyLevels(filename);

        var flashCount = 0;
        for (var step = 0; step < 100; step++)
        {
            flashCount += CountFlashesByPerformingStep(ref energyLevels);
        }

        Assert.Equal(expectation, flashCount);
    }

    private static async Task<int[]> ReadInputEnergyLevels(string filename)
    {
        return (await Input.ReadAllLinesAsync(filename)).SelectMany(line => line.Select(c => int.Parse(c.ToString()))).ToArray();
    }

    [Theory]
    [InlineData("day11_example.txt", 195)]
    [InlineData("day11.txt", 502)]
    public async void Part2(string filename, int expectation)
    {
        var energyLevels = await ReadInputEnergyLevels(filename);

        var stepDidAllFlash = default(int?);
        for (var step = 0; !stepDidAllFlash.HasValue; step++)
        {
            if (CountFlashesByPerformingStep(ref energyLevels) == 100)
            {
                stepDidAllFlash = step;
            }
        }

        Assert.Equal(expectation, stepDidAllFlash.Value + 1);
    }

    private static int CountFlashesByPerformingStep(ref int[] energyLevels)
    {
        // First, the energy level of each octopus increases by 1.
        energyLevels = energyLevels.Select(el => el + 1).ToArray();

        // Then, any octopus with an energy level greater than 9 FLASHES.
        var flasherIndices = energyLevels.Select((el, i) => el > 9 ? i : default(int?)).Where(i => i.HasValue).Cast<int>().ToHashSet();

        var totalFlasherIndices = new HashSet<int>();

        while (flasherIndices.Any())
        {
            totalFlasherIndices.UnionWith(flasherIndices);

            // This increases the energy level of all adjacent octopuses by 1, including octopuses that are diagonally adjacent.
            var flasherAdjacentIndices = flasherIndices.SelectMany(fi =>
            {
                var x = fi % 10;
                var y = fi / 10;

                var idxs = new List<int>();
                if (fi > 0)
                {
                    idxs.Add(fi - 10);
                }

                var ints = new int?[]
                {
                    x == 0 || y == 0 ? null : fi - 11, y == 0 ? null : fi - 10, x == 9 || y == 0 ? null : fi - 9,
                    x == 0 ? null : fi - 1, /*         fi,                   */ x == 9 ? null : fi + 1,
                    x == 0 || y == 9 ? null : fi + 9, y == 9 ? null : fi + 10, x == 9 || y == 9 ? null : fi + 11,
                };
                return ints.Where(optInt => optInt.HasValue).Cast<int>();
            }).Where(i => i is >= 0 and < 100).ToArray();
            foreach (var flasherAdjacentIndex in flasherAdjacentIndices)
            {
                energyLevels[flasherAdjacentIndex]++;
            }

            // If this causes an octopus to have an energy level greater than 9, it ALSO FLASHES.

            flasherIndices.Clear();
            foreach (var fai in flasherAdjacentIndices.Except(totalFlasherIndices))
            {
                if (energyLevels[fai] > 9)
                {
                    flasherIndices.Add(fai);
                }
            }

            //flasherIndices = flasherAdjacentIndices.Except(totalFlasherIndices).Where(fai => energyLevels[fai] > 9).ToHashSet();
        }

        // This process continues as long as new octopuses keep having their energy level increased beyond 9.
        // (An octopus can only flash AT MOST ONCE PER STEP.)

        // Finally, any octopus that flashed during this step has its energy level set to 0, as it used all of its energy to flash.
        foreach (var flasherIndex in totalFlasherIndices)
        {
            energyLevels[flasherIndex] = 0;
        }

        return totalFlasherIndices.Count;
    }
}