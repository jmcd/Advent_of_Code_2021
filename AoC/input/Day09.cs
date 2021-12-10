namespace AoC.input;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day09
{
    [Theory]
    [InlineData("day09_example.txt", 15)]
    [InlineData("day09.txt", 528)]
    public async Task Part1(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var heightMap = await ReadInputIntoHeightMapAsync(filename);

        var riskSum = heightMap.GetLowPoints().Sum(point => heightMap[point] + 1);

        Assert.Equal(expectation, riskSum);
    }

    [Theory]
    [InlineData("day09_example.txt", 1134)]
    [InlineData("day09.txt", 920448)]
    public async Task Part2(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var heightMap = await ReadInputIntoHeightMapAsync(filename);

        var lowPoints = heightMap.GetLowPoints();

        var result = lowPoints
            .Select(lowPoint => heightMap.GetBasin(lowPoint))
            .Select(basin => basin.Count)
            .OrderByDescending(i => i)
            .Take(3)
            .Aggregate(1, (current, size) => current * size);

        Assert.Equal(expectation, result);
    }

    private static async Task<HeightMap> ReadInputIntoHeightMapAsync(string filename)
    {
        var rowsOfColumns = (await Input.ReadAllLinesAsync(filename))
            .Select(line =>
                line.Select(c => int.Parse(c.ToString())).ToArray()
            ).ToArray();
        return new HeightMap(rowsOfColumns);
    }
}

public readonly record struct Point(int X, int Y);

public record HeightMap(int[][] RowsOfColumns)
{
    private int ColumnCount { get; } = RowsOfColumns[0].Length;

    private int RowCount { get; } = RowsOfColumns.Length;

    private int[][] RowsOfColumns { get; } = RowsOfColumns;

    public int this[Point point] => RowsOfColumns[point.Y][point.X];

    private IEnumerable<Point> GetAdjacentPoints(Point point)
    {
        if (point.Y > 0)
        {
            yield return new Point(point.X, point.Y - 1);
        }
        if (point.Y < RowCount - 1)
        {
            yield return new Point(point.X, point.Y + 1);
        }
        if (point.X > 0)
        {
            yield return new Point(point.X - 1, point.Y);
        }
        if (point.X < ColumnCount - 1)
        {
            yield return new Point(point.X + 1, point.Y);
        }
    }

    public IEnumerable<Point> GetLowPoints()
    {
        for (var y = 0; y < RowCount; y++)
        {
            for (var x = 0; x < ColumnCount; x++)
            {
                var point = new Point(x, y);
                var height = this[point];

                var isLowPoint = GetAdjacentPoints(point).All(p => height < this[p]);

                if (isLowPoint)
                {
                    yield return point;
                }
            }
        }
    }

    public ISet<Point> GetBasin(Point centre)
    {
        var basin = new HashSet<Point> { centre };
        var prev = new HashSet<Point> { centre };
        do
        {
            var next = new HashSet<Point>();
            foreach (var point in prev)
            {
                var qualifyingAdjacentPoints = GetAdjacentPoints(point).Where(p => !prev.Contains(p) && this[p] > this[point] && this[p] < 9);
                foreach (var adjacentPoint in qualifyingAdjacentPoints)
                {
                    next.Add(adjacentPoint);
                    basin.Add(adjacentPoint);
                }
            }
            prev = next;
        } while (prev.Count > 0);

        return basin;
    }
}