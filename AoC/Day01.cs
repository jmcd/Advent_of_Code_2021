namespace AoC;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day01
{
    [InlineData("day01_example.txt", 7)]
    [InlineData("day01.txt", 1832)]
    [Theory]
    public async Task Part1(string filename, int expectation)
    {
        var depths = await LoadDepthsAsync(filename);

        var count = CountOfValueGreaterThanPrevious(depths);

        Assert.Equal(count, expectation);
    }

    [InlineData("day01_example.txt", 5)]
    [InlineData("day01.txt", 1858)]
    [Theory]
    public async Task Part2(string filename, int expectation)
    {
        var depths = await LoadDepthsAsync(filename);

        var windows = MakeSlidingWindows(3, depths);

        var count = CountOfValueGreaterThanPrevious(windows);

        Assert.Equal(count, expectation);
    }

    private static async Task<IEnumerable<int>> LoadDepthsAsync(string filename) => (await Input.ReadAllLinesAsync(filename)).Select(int.Parse);

    private static int CountOfValueGreaterThanPrevious(IEnumerable<int> depths)
    {
        var count = 0;

        var prevDepth = default(int?);

        foreach (var depth in depths)
        {
            if (depth > prevDepth)
            {
                count += 1;
            }

            prevDepth = depth;
        }

        return count;
    }

    private IEnumerable<int> MakeSlidingWindows_Orig(int windowLength, IEnumerable<int> values)
    {
        var window = new List<int>();

        foreach (var depth in values)
        {
            if (window.Count == windowLength)
            {
                window.RemoveAt(0);
            }

            window.Add(depth);
            if (window.Count == windowLength)
            {
                yield return window.Sum();
            }
        }
    }

    // A second, "fancier" impl. using a fixed size array. Sacrifices readability for cleverness? 🤔
    private IEnumerable<int> MakeSlidingWindows(int windowLength, IEnumerable<int> values)
    {
        var window = new int[windowLength];
        var i = 0;
        var iterationCount = 0;

        foreach (var depth in values)
        {
            window[i = (i + 1) % windowLength] = depth;
            if ((iterationCount += 1) >= windowLength)
            {
                yield return window.Sum();
            }
        }
    }
}