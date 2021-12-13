namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day13
{
    [Theory]
    [InlineData("day13_example.txt", 17)]
    [InlineData("day13.txt", 729)]
    public async Task Part1(string filename, int expectation)
    {
        var (dots, folds) = await ReadInput(filename);

        var fold = folds[0];

        for (var i = 0; i < dots.Count; i++)
        {
            dots[i] = dots[i].Folding(fold);
        }

        var map = CreateDotMap(dots);

        var visibleDotCount = map.Count(b => b);

        Assert.Equal(visibleDotCount, expectation);
    }

    private static bool[] CreateDotMap(List<Dot> dots)
    {
        var width = dots.Select(d => d.X).Max() + 1;
        var height = dots.Select(d => d.Y).Max() + 1;
        var map = new bool[width * height];
        foreach (var i in dots.Select(dot => dot.Y * width + dot.X))
        {
            map[i] = true;
        }
        return map;
    }

    private static async Task<(List<Dot> dots, List<Fold> folds)> ReadInput(string filename)
    {
        var lines = await Input.ReadAllLinesAsync(filename);

        var dots = new List<Dot>();
        var folds = new List<Fold>();

        var haveReadBlankLine = false;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) { haveReadBlankLine = true; }
            else if (!haveReadBlankLine) { dots.Add(Dot.Parsing(line)); }
            else { folds.Add(Fold.Parsing(line)); }
        }
        return (dots, folds);
    }

    private enum Axis
    {
        X,
        Y,
    }

    private record struct Dot(int X, int Y)
    {
        public static Dot Parsing(string inputLine)
        {
            var comps = inputLine.Split(",");
            return new(int.Parse(comps[0]), int.Parse(comps[1]));
        }

        public Dot Folding(Fold fold)
        {
            var dot = this;
            switch (fold.Axis)
            {
                case Axis.X:
                    if (dot.X > fold.Location) { dot.X = fold.Location - (dot.X - fold.Location); }
                    break;
                case Axis.Y:
                    if (dot.Y > fold.Location) { dot.Y = fold.Location - (dot.Y - fold.Location); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return dot;
        }
    }

    private record struct Fold(Axis Axis, int Location)
    {
        public static Fold Parsing(string inputLine)
        {
            Axis AxisFrom(char c) =>
                c switch
                {
                    'x' => Axis.X,
                    'y' => Axis.Y,
                    _ => throw new ArgumentException($"Cannot convert {c} to Axis", nameof(c)),
                };

            var axisCharLocation = "fold along ".Length;
            var axisChar = inputLine[axisCharLocation];
            var axis = AxisFrom(axisChar);
            var location = int.Parse(inputLine[(axisCharLocation + 2)..]);
            return new Fold(axis, location);
        }
    }
}