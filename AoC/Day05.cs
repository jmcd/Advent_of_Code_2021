namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day05
{
    public enum Strategy
    {
        ExcludeDiagonal, // Part 1
        IncludeDiagonal, // Part 2
    }

    [Theory]
    [InlineData("day05_example.txt", Strategy.ExcludeDiagonal, 5)]
    [InlineData("day05.txt", Strategy.ExcludeDiagonal, 8622)]
    [InlineData("day05_example.txt", Strategy.IncludeDiagonal, 12)]
    [InlineData("day05.txt", Strategy.IncludeDiagonal, 22037)]
    public async Task Part1And2(string filename, Strategy strategy, int expectation)
    {
        var inputPoints = (await Input.ReadAllLinesAsync(filename)).Select(ParseLine);

        var pointToCount = new Dictionary<(int, int), int>();

        foreach (var (x1, y1, x2, y2) in inputPoints)
        {
            var yDiff = y2 - y1;
            var xDiff = x2 - x1;

            var yInc = Math.Sign(yDiff);
            var xInc = Math.Sign(xDiff);

            if (strategy == Strategy.ExcludeDiagonal && yDiff != 0 && xDiff != 0)
            {
                continue;
            }

            var pFirst = (x: x1, y: y1);
            var pLast = (x: x2, y: y2);

            var p = pFirst;
            IncrementPointCount(pointToCount, p);
            while (p != pLast)
            {
                p = (p.x + xInc, p.y + yInc);
                IncrementPointCount(pointToCount, p);
            }
        }

        var countOfPointsWhereAtLeast2LinesOverlap = pointToCount.Values.Count(v => v > 1);
        Assert.Equal(expectation, countOfPointsWhereAtLeast2LinesOverlap);
    }

    private static void IncrementPointCount(IDictionary<(int x, int y), int> pointToCount, (int x, int y) point)
    {
        if (pointToCount.TryGetValue(point, out var currentCount))
        {
            pointToCount[point] = currentCount + 1;
        }
        else
        {
            pointToCount[point] = 1;
        }
    }

    private (int x1, int y1, int x2, int y2) ParseLine(string line)
    {
        var numbers = line.Split(new[] { ",", " -> " }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        return (x1: numbers[0], y1: numbers[1], x2: numbers[2], y2: numbers[3]);
    }
}