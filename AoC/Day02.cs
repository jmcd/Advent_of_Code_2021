namespace AoC;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day02
{
    private static async Task<IEnumerable<(string command, int magnitude)>> ReadInput(string filename) =>
        (await Input.ReadAllLinesAsync(filename)).Select(line =>
        {
            var comps = line.Split(" ");
            return (comps[0], int.Parse(comps[1]));
        });

    [Theory]
    [InlineData("day02_example.txt", 150)]
    [InlineData("day02.txt", 1692075)]
    public async Task Part1(string filename, int expectation)
    {
        var (x, y) = (0, 0);
        foreach (var (command, magnitude) in await ReadInput(filename))
        {
            var (signX, signY) = command switch
            {
                "forward" => (1, 0),
                "up" => (0, -1),
                "down" => (0, 1),
                _ => (0, 0),
            };
            x += signX * magnitude;
            y += signY * magnitude;
        }

        Assert.Equal(x * y, expectation);
    }

    [Theory]
    [InlineData("day02_example.txt", 900)]
    [InlineData("day02.txt", 1749524700)]
    public async Task Part2(string filename, int expectation)
    {
        var (x, y, aim) = (0, 0, 0);
        foreach (var (command, magnitude) in await ReadInput(filename))
        {
            switch (command)
            {
                case "down":
                    aim += magnitude;
                    break;
                case "up":
                    aim -= magnitude;
                    break;
                case "forward":
                    x += magnitude;
                    y += aim * magnitude;
                    break;
            }
        }

        Assert.Equal(x * y, expectation);
    }
}