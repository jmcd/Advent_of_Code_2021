namespace AoC;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public class Day12
{
    private ITestOutputHelper _op;
    public Day12(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("day12_example1.txt", 10)]
    [InlineData("day12_example2.txt", 19)]
    [InlineData("day12_example3.txt", 226)]
    [InlineData("day12.txt", 4691)]
    public async void Part1(string filename, int expectation)
    {
        Debug.WriteLine(filename, expectation);

        var lines = await Input.ReadAllLinesAsync(filename);

        var allConnections = lines.Select(line =>
        {
            var caveNames = line.Split("-");
            return new Connection(caveNames[0], caveNames[1]);
        }).ToArray();

        var paths = FindDistinctPaths(allConnections);

        Assert.Equal(expectation, paths.Count);
    }

    private static List<string[]> FindDistinctPaths(Connection[] allConnections)
    {
        var completedPaths = new List<string[]>();
        FindDistinctPaths(new[] { "start" }, allConnections, completedPaths);
        return completedPaths;
    }

    private static void FindDistinctPaths(string[] path, Connection[] allConnections, ICollection<string[]> completedPaths)
    {
        var caveName = path[^1];
        if (caveName == "end")
        {
            completedPaths.Add(path);
            return;
        }

        var unvisitableCaves = path.Where(vc => char.IsLower(vc[0]));

        var exitCaves = allConnections
            .Where(p => p.HasCave(caveName))
            .Select(p => p.OtherCave(caveName))
            .Except(unvisitableCaves);

        foreach (var exitCave in exitCaves)
        {
            var continuedPath = path.Append(exitCave).ToArray();
            FindDistinctPaths(continuedPath, allConnections, completedPaths);
        }
    }

    private readonly record struct Connection(string LeftCave, string RightCave)
    {
        public bool HasCave(string name) => LeftCave == name || RightCave == name;

        public string OtherCave(string name)
        {
            if (name == LeftCave) { return RightCave; }
            if (name == RightCave) { return LeftCave; }
            throw new ArgumentException($"No such cave '{name}'", nameof(name));
        }
    }
}