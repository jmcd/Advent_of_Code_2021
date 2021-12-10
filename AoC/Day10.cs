namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class Day10
{
    [Theory]
    [InlineData("day10_example.txt", 26397)]
    [InlineData("day10.txt", 26397)]
    public async void Part1(string filename, int expectation)
    {
        var lines = await Input.ReadAllLinesAsync(filename);

        const string openers = "([{<";
        const string closers = ")]}>";

        var openerToCloser = openers.Zip(closers).ToDictionary(oc => oc.First, oc => oc.Second);

        var score = 0;
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
                    if (c != expectedCloser)
                    {
                        corruptingChar = c;
                        break;
                    }
                }
            }
            score += Score(corruptingChar) ?? 0;
        }
        Assert.Equal(expectation, score);
    }

    private static int? Score(char? corruptingChar)
    {
        return corruptingChar switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            null => null,
            _ => throw new ArgumentException(null, nameof(corruptingChar)),
        };
    }
}