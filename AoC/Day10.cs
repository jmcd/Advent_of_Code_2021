namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class Day10
{
    [Theory]
    [InlineData("day10_example.txt", 26397, 288957)]
    [InlineData("day10.txt", 323613, 3103006161)]
    public async void Part1And2(string filename, int expectedSyntaxErrorScoreForPart1, long expectedMiddleCompletionScoreForPart2)
    {
        var lines = await Input.ReadAllLinesAsync(filename);

        const string openers = "([{<";
        const string closers = ")]}>";

        var openerToCloser = openers.Zip(closers).ToDictionary(oc => oc.First, oc => oc.Second);

        var syntaxErrorScore = 0;
        var completionScores = new List<long>();

        foreach (var line in lines)
        {
            var corruptingChar = default(char?);
            var stack = new Stack<char>();

            foreach (var c in line)
            {
                if (openerToCloser.ContainsKey(c))
                {
                    stack.Push(c);
                }
                else
                {
                    var opener = stack.Pop();
                    var expectedCloser = openerToCloser[opener];
                    if (c == expectedCloser)
                    {
                        continue;
                    }
                    corruptingChar = c;
                    break;
                }
            }

            if (corruptingChar.HasValue)
            {
                syntaxErrorScore += SyntaxErrorScore(corruptingChar.Value);
            }
            else
            {
                var lineCompletionScore = 0L;
                while (stack.TryPop(out var opener))
                {
                    var closer = openerToCloser[opener];
                    lineCompletionScore = lineCompletionScore * 5 + CompletionScore(closer);
                }
                completionScores.Add(lineCompletionScore);
            }
        }

        completionScores.Sort();
        var middleCompletionScore = completionScores[completionScores.Count / 2];

        Assert.Equal(expectedSyntaxErrorScoreForPart1, syntaxErrorScore);
        Assert.Equal(expectedMiddleCompletionScoreForPart2, middleCompletionScore);
    }

    private static int SyntaxErrorScore(char corruptingChar)
    {
        return corruptingChar switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new ArgumentException(null, nameof(corruptingChar)),
        };
    }

    private static long CompletionScore(char closer)
    {
        return closer switch
        {
            ')' => 1,
            ']' => 2,
            '}' => 3,
            '>' => 4,
            _ => throw new ArgumentException(null, nameof(closer)),
        };
    }
}