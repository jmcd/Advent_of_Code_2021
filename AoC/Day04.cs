namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day04
{
    public enum Strategy
    {
        StopAfterFirstWin, // Part 1
        StopAfterLastWin, // Part 2
    }

    [Theory]
    [InlineData("day04_example.txt", Strategy.StopAfterFirstWin, 188 * 24)]
    [InlineData("day04.txt", Strategy.StopAfterFirstWin, 932 * 48)]
    [InlineData("day04_example.txt", Strategy.StopAfterLastWin, 148 * 13)]
    [InlineData("day04.txt", Strategy.StopAfterLastWin, 261 * 7)]
    public async Task Part1And2(string filename, Strategy strategy, int expectation)
    {
        var lines = (await Input.ReadAllLinesAsync(filename)).ToList();

        var numbersThatWillBeDrawn = lines[0].Split(",").Select(int.Parse).ToList();

        var boardNumbers = lines.Skip(1).SelectMany(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)).ToArray();

        var numberToIndex = CreateNumberToIndex(boardNumbers);

        var indexOfLastBoardMarkedAsWinning = default(int?);
        var boardIndexToWinningScore = new Dictionary<int, int>();

        var numbersDrawnSoFar = new HashSet<int>();
        var indicesMarkedSoFar = new HashSet<int>();

        foreach (var drawnNumber in numbersThatWillBeDrawn)
        {
            numbersDrawnSoFar.Add(drawnNumber);

            if (!numberToIndex.TryGetValue(drawnNumber, out var indicesOfDrawnNumber))
            {
                continue;
            }

            indicesMarkedSoFar.UnionWith(indicesOfDrawnNumber);

            foreach (var indexOfDrawnNumber in indicesOfDrawnNumber)
            {
                var boardIndex = indexOfDrawnNumber / 25;
                var indexLocalToBoard = indexOfDrawnNumber % 25;
                var rowIndex = indexLocalToBoard / 5;
                var colIndex = indexLocalToBoard % 5;

                if (boardIndexToWinningScore.ContainsKey(boardIndex) ||
                    // !Win by row
                    !new[] { 0, 1, 2, 3, 4 }.All(i => indicesMarkedSoFar.Contains(boardIndex * 25 + rowIndex * 5 + i)) &&
                    // !Win by column
                    !new[] { 0, 5, 10, 15, 20 }.All(i => indicesMarkedSoFar.Contains(boardIndex * 25 + colIndex + i)))
                {
                    continue;
                }

                var allUnmarkedNumbersOnBoard = boardNumbers.Skip(boardIndex * 25).Take(25).Except(numbersDrawnSoFar);
                var sumOfAllUnmarkedNumbersOnBoard = allUnmarkedNumbersOnBoard.Sum();
                var score = sumOfAllUnmarkedNumbersOnBoard * drawnNumber;

                indexOfLastBoardMarkedAsWinning = boardIndex;
                boardIndexToWinningScore[boardIndex] = score;

                if (strategy == Strategy.StopAfterFirstWin)
                {
                    goto outerLoop;
                }
            }
        }
        outerLoop:

        var winningScore = boardIndexToWinningScore[indexOfLastBoardMarkedAsWinning!.Value];
        Assert.Equal(expectation, winningScore);
    }

    private static Dictionary<int, ISet<int>> CreateNumberToIndex(IReadOnlyList<int> boardNumbers)
    {
        var numberToIndex = new Dictionary<int, ISet<int>>();
        for (var i = 0; i < boardNumbers.Count; i++)
        {
            var n = boardNumbers[i];
            ISet<int> set;
            if (numberToIndex.TryGetValue(n, out var s))
            {
                set = s;
            }
            else
            {
                set = new HashSet<int>();
                numberToIndex[n] = set;
            }
            set.Add(i);
        }
        return numberToIndex;
    }
}