namespace AoC;

using System;
using System.Linq;
using Xunit;

public class Day17
{
    [Theory]
    [InlineData("target area: x=20..30, y=-10..-5", 45, 112)]
    [InlineData("target area: x=169..206, y=-108..-68", 5778, 2576)]
    public void Part1And2(string input, int part1Expectation, int part2Expectation)
    {
        var inputNumbers = input.Split(new[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.Parse(string.Join("", s.Where(c => c == '-' || char.IsDigit(c)))))
            .ToArray();

        var targetArea = (
            x: (min: inputNumbers[0], max: inputNumbers[1]),
            y: (min: inputNumbers[2], max: inputNumbers[3]));

        bool IsWithinTargetArea((int x, int y) position) =>
            position.x >= targetArea.x.min && position.x <= targetArea.x.max &&
            position.y >= targetArea.y.min && position.y <= targetArea.y.max;

        var largestMagnitudes = (
            x: Math.Max(Math.Abs(targetArea.x.min), Math.Abs(targetArea.x.max)),
            y: Math.Max(Math.Abs(targetArea.y.min), Math.Abs(targetArea.y.max)));

        var largestYPositionAcrossAllVelocities = 0;
        var numberOfDistinctInitialVelocities = 0;

        for (var initialYVelocity = -largestMagnitudes.y; initialYVelocity <= largestMagnitudes.y; initialYVelocity++)
        {
            for (var initialXVelocity = -largestMagnitudes.x; initialXVelocity <= largestMagnitudes.x; initialXVelocity++)
            {
                var velocity = (x: initialXVelocity, y: initialYVelocity);
                var position = (x: 0, y: 0);
                var largestYPosition = 0;

                while (position.x <= targetArea.x.max && position.y >= targetArea.y.min)
                {
                    position = (position.x + velocity.x, position.y + velocity.y);
                    velocity = (velocity.x - Math.Sign(velocity.x), velocity.y - 1);

                    largestYPosition = Math.Max(largestYPosition, position.y);

                    if (IsWithinTargetArea(position))
                    {
                        largestYPositionAcrossAllVelocities = Math.Max(largestYPositionAcrossAllVelocities, largestYPosition);
                        numberOfDistinctInitialVelocities += 1;
                        break;
                    }
                }
            }
        }
        Assert.Equal(part1Expectation, largestYPositionAcrossAllVelocities);
        Assert.Equal(part2Expectation, numberOfDistinctInitialVelocities);
    }
}