namespace AoC;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day07
{
    public enum StrategyKind
    {
        Linear, // Part 1
        NonLinear, // Part 2
    }

    [Theory]
    [InlineData("day07_example.txt", StrategyKind.Linear, 37)]
    [InlineData("day07.txt", StrategyKind.Linear, 328318)]
    [InlineData("day07_example.txt", StrategyKind.NonLinear, 168)]
    [InlineData("day07.txt", StrategyKind.NonLinear, 89791146)]
    public async Task Part1And2(string filename, StrategyKind strategyKind, int expectation)
    {
        var positions = (await Input.ReadSingleLineAsync(filename)).Split(",").Select(int.Parse).ToArray();

        var maxPosition = positions.Max();

        var costStrategy = strategyKind == StrategyKind.Linear
            ? (ICostStrategy)new LinearCostStrategy()
            : new NonLinearCostStrategy(maxPosition);

        var bestCost = default(int?);
        for (var targetPosition = 0; targetPosition < maxPosition; targetPosition++)
        {
            var cost = positions.Sum(position => costStrategy.GetCost(targetPosition, position));
            bestCost = bestCost.HasValue ? Math.Min(cost, bestCost.Value) : cost;
        }

        Assert.Equal(expectation, bestCost!.Value);
    }

    private interface ICostStrategy
    {
        int GetCost(int targetPosition, int position);
    }

    private struct LinearCostStrategy : ICostStrategy
    {
        public int GetCost(int targetPosition, int position) => Math.Abs(position - targetPosition);
    }

    private readonly struct NonLinearCostStrategy : ICostStrategy
    {
        private readonly int[] costByDistance;

        public NonLinearCostStrategy(int maxPosition)
        {
            var ar = new int[maxPosition + 1];
            for (var i = 1; i < ar.Length; i++)
            {
                ar[i] = ar[i - 1] + i;
            }

            costByDistance = ar;
        }

        public int GetCost(int targetPosition, int position) => costByDistance[Math.Abs(position - targetPosition)];
    }
}