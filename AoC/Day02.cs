namespace AoC;

using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day02
{
    [Theory]
    [InlineData("day02_example.txt", 150)]
    [InlineData("day02.txt", 1692075)]
    public async Task Part1(string filename, int expectation)
    {
        var deltas = (await Input.ReadAllLinesAsync(filename)).Select(ParseCommandDelta);

        var position = (x: 0, y: 0);
        foreach (var delta in deltas)
        {
            position.x += delta.x;
            position.y += delta.y;
        }

        Assert.Equal(position.x * position.y, expectation);
    }

    private static (int x, int y) ParseCommandDelta(string s)
    {
        // There is some matrix multiplication thing that escapes me?
        var comps = s.Split(" ");
        (int x, int y) sign = comps[0] switch
        {
            "forward" => (1, 0),
            "up" => (0, -1),
            "down" => (0, 1),
            _ => (0, 0),
        };

        var magnitude = int.Parse(comps[1]);
        return (sign.x * magnitude, sign.y * magnitude);
    }

    [Theory]
    [InlineData("day02_example.txt", 900)]
    [InlineData("day02.txt", 1749524700)]
    public async Task Part2(string filename, int expectation)
    {
        var lines = await Input.ReadAllLinesAsync(filename);

        var position = (x: 0, y: 0, aim: 0);

        foreach (var line in lines)
        {
            var comps = line.Split(" ");
            var magnitude = int.Parse(comps[1]);
            switch (comps[0])
            {
                case "down":
                    position.aim += magnitude;
                    break;
                case "up":
                    position.aim -= magnitude;
                    break;
                case "forward":
                    position.x += magnitude;
                    position.y += position.aim * magnitude;
                    break;
            }
        }

        Assert.Equal(position.x * position.y, expectation);
    }
}