namespace AoC;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public class Day11
{
    private ITestOutputHelper _op;

    public Day11(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("day11_example.txt", 1656)]
    [InlineData("day11.txt", 1713)]
    public async void Part1(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var energyLevels = (await Input.ReadAllLinesAsync(filename)).SelectMany(line => line.Select(c => int.Parse(c.ToString()))).ToArray();

        Print("Before any steps:", energyLevels);
        var flashCount = 0;
        for (var step = 0; step < 100; step++)
        {
            // First, the energy level of each octopus increases by 1.
            energyLevels = energyLevels.Select(el => el + 1).ToArray();

            // Then, any octopus with an energy level greater than 9 FLASHES.
            var flasherIndices = energyLevels.Select((el, i) => el > 9 ? i : default(int?)).Where(i => i.HasValue).Cast<int>().ToHashSet();

            var totalFlasherIndices = new HashSet<int>();

            while (flasherIndices.Any())
            {
                _op.WriteLine($"Step {step + 1}: {string.Join(", ", flasherIndices)}");

                Print($"Step {step + 1}:", energyLevels);

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
                flasherIndices = flasherAdjacentIndices.Except(totalFlasherIndices).Where(fai => energyLevels[fai] > 9).ToHashSet();
            }

            // This process continues as long as new octopuses keep having their energy level increased beyond 9.
            // (An octopus can only flash AT MOST ONCE PER STEP.)

            // Finally, any octopus that flashed during this step has its energy level set to 0, as it used all of its energy to flash.
            foreach (var flasherIndex in totalFlasherIndices)
            {
                energyLevels[flasherIndex] = 0;
            }

            flashCount += totalFlasherIndices.Count;

            Print($"After step {step + 1}:", energyLevels);
        }

        Assert.Equal(expectation, flashCount);
    }

    private void Print(string description, IEnumerable<int> energyLevels)
    {
        _op.WriteLine(description);
        var els = energyLevels.ToArray();
        var row = "";
        for (var i = 0; i < els.Length; i++)
        {
            var el = els[i];
            row += el < 10 ? el.ToString() : "*";
            if ((i + 1) % 10 == 0)
            {
                _op.WriteLine(row);
                row = "";
            }
        }
        _op.WriteLine("");
    }
}