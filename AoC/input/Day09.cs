namespace AoC.input;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Day09
{
    private ITestOutputHelper _op;
    public Day09(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("day09_example.txt", 15)]
    [InlineData("day09.txt", 528)]
    public async Task Part1(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var heights = (await Input.ReadAllLinesAsync(filename)).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

        Assert.True(heights.All(x => x.Length == heights[0].Length));

        _op.WriteLine($"{heights[0].Length} {heights.Length}");

        var rowCount = heights.Length;
        var columnCount = heights[0].Length;
        var riskSum = 0;
        for (var y = 0; y < rowCount; y++)
        {
            for (var x = 0; x < columnCount; x++)
            {
                var height = heights[y][x];

                var isLowerThanUp = y == 0 || height < heights[y - 1][x];
                var isLowerThanDown = y == rowCount - 1 || height < heights[y + 1][x];
                var isLowerThanLeft = x == 0 || height < heights[y][x - 1];
                var isLowerThanRight = x == columnCount - 1 || height < heights[y][x + 1];

                var lowPoint = isLowerThanUp && isLowerThanDown && isLowerThanLeft && isLowerThanRight;

                if (lowPoint)
                {
                    var risk = height + 1;
                    riskSum += risk;
                }
            }
        }
        Assert.Equal(expectation, riskSum);
    }
}