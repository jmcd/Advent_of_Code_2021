namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day12
{
    [Theory]
    [InlineData("day12_example1.txt", 10)]
    [InlineData("day12_example2.txt", 19)]
    [InlineData("day12_example3.txt", 226)]
    [InlineData("day12.txt", 4691)]
    public async void Part1(string filename, int expectation)
    {
        var connections = await ReadInputConnections(filename);
        var paths = FindDistinctPaths(connections);
        Assert.Equal(expectation, paths.Count);
    }

    [Theory]
    [InlineData("day12_example1.txt", 36)]
    [InlineData("day12_example2.txt", 103)]
    [InlineData("day12_example3.txt", 3509)]
    [InlineData("day12.txt", 140718)]
    public async void Part2(string filename, int expectation)
    {
        var connections = await ReadInputConnections(filename);
        var paths = FindDistinctPaths_AllowingForVisitingSingleSmallCaveTwice(connections);
        Assert.Equal(expectation, paths.Count);
    }

    private static List<string[]> FindDistinctPaths(Connection[] allConnections)
    {
        void Recurse(string[] path, ICollection<string[]> completedPaths)
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
                Recurse(continuedPath, completedPaths);
            }
        }

        var completedPaths = new List<string[]>();
        Recurse(new[] { "start" }, completedPaths);
        return completedPaths;
    }

    private static List<string[]> FindDistinctPaths_AllowingForVisitingSingleSmallCaveTwice(Connection[] allConnections)
    {
        void Recurse(string[] path, ISet<string[]> completedPaths)
        {
            var caveName = path[^ 1];
            if (caveName == "end")
            {
                completedPaths.Add(path);
                return;
            }

            var exitCaves = allConnections
                .Where(p => p.HasCave(caveName))
                .Select(p => p.OtherCave(caveName))
                .ToArray();

            var exitSmallCaves = exitCaves.Where(ec => ec != "start" && char.IsLower(ec[0])).ToArray();

            var visitedSmallCaves = path.Where(vc => char.IsLower(vc[0])).ToArray();

            exitCaves = exitCaves.Except(visitedSmallCaves).ToArray();

            var canVisitASmallCave = visitedSmallCaves.Aggregate(new Dictionary<string, int>(), (caveToCount, cave) =>
            {
                if (!caveToCount.TryGetValue(cave, out var currentCount)) { currentCount = 0; }
                caveToCount[cave] = currentCount + 1;
                return caveToCount;
            }).All(kvp => kvp.Value == 1);

            if (canVisitASmallCave)
            {
                exitCaves = exitCaves.Union(exitSmallCaves).ToArray();
            }

            foreach (var exitCave in exitCaves)
            {
                var continuedPath = path.Append(exitCave).ToArray();
                Recurse(continuedPath, completedPaths);
            }
        }

        var completedPaths = new HashSet<string[]>();
        Recurse(new[] { "start" }, completedPaths);
        return completedPaths.ToList();
    }

    private static async Task<Connection[]> ReadInputConnections(string filename) =>
        (await Input.ReadAllLinesAsync(filename)).Select(line =>
        {
            var caveNames = line.Split("-");
            return new Connection(caveNames[0], caveNames[1]);
        }).ToArray();

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