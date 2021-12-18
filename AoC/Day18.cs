namespace AoC;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

public class Day18
{
    private ITestOutputHelper _op;
    public Day18(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
    [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
    [InlineData("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
    [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
    [InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
    public void TestSingleExplode(string input, string expectation)
    {
        Debug.WriteLine(expectation);

        _op.WriteLine($"input: {input}");

        var tokens = new Tokenizer(input).Tokenize();

        var level = 0;
        var leftNumberIndex = default(int?);
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];
            switch (token.TokenKind)
            {
                case TokenKind.Separator:
                    break;
                case TokenKind.Number:
                    if (level == 5)
                    {
                        Assert.Equal(TokenKind.Separator, tokens[index + 1].TokenKind);
                        Assert.Equal(TokenKind.Number, tokens[index + 2].TokenKind);
                        Assert.Equal(TokenKind.Closer, tokens[index + 3].TokenKind);
                        var rightNumberIndex = default(int?);
                        for (var i = index + 4; i < tokens.Count; i++)
                        {
                            if (tokens[i].TokenKind == TokenKind.Number)
                            {
                                rightNumberIndex = i;
                                break;
                            }
                        }

                        if (leftNumberIndex.HasValue)
                        {
                            _op.WriteLine($"add left value ({token.Body}) to {tokens[leftNumberIndex.Value].Body}");
                            var n0 = int.Parse(token.Body);
                            var n1 = int.Parse(tokens[leftNumberIndex.Value].Body);
                            tokens[leftNumberIndex.Value] = new((n0 + n1).ToString(), TokenKind.Number);
                        }

                        if (rightNumberIndex.HasValue)
                        {
                            _op.WriteLine($"add right value ({tokens[index + 2].Body}) to {tokens[rightNumberIndex.Value].Body}");
                            var n0 = int.Parse(tokens[index + 2].Body);
                            var n1 = int.Parse(tokens[rightNumberIndex.Value].Body);
                            tokens[rightNumberIndex.Value] = new((n0 + n1).ToString(), TokenKind.Number);
                        }

                        _op.WriteLine($"replace tokens {tokens[index - 1].Body} {tokens[index].Body} {tokens[index + 1].Body} {tokens[index + 2].Body} {tokens[index + 3].Body} with 0");

                        for (var i = 3; i >= -1; i -= 1)
                        {
                            tokens.RemoveAt(index + i);
                        }
                        tokens.Insert(index-1, new("0", TokenKind.Number));


                        goto outer;
                    }
                    else
                    {
                        leftNumberIndex = index;
                    }
                    break;
                case TokenKind.Opener:
                    level += 1;
                    break;
                case TokenKind.Closer:
                    level -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        outer:

        var actual = tokens.Aggregate(new StringBuilder(), (builder, token) => builder.Append(token.Body)).ToString();
        Assert.Equal(expectation, actual);

        // var parser = new Parser(input);
        // var pair = parser.Parse();
        //
        //
        //
        //
        // _op.WriteLine(input);
        // Assert.Equal(expectation, pair.ToString());
    }

    private void Explode3(Pair pair)
    {
        var root = pair;
        while (root.Parent is not null)
        {
            root = root.Parent;

            if (root.Left is RegularNumber l)
            {
                _op.WriteLine($"found left {l}");
            }
            if (root.Right is RegularNumber r)
            {
                _op.WriteLine($"found right {r}");
            }
        }

        _op.WriteLine($"root is {root}");

        //
        //
        // _op.WriteLine($"explode {pair}");
        //
        // var rootLeftValue = (RegularNumber)pair.Left;
        // var rightValue = (RegularNumber)pair.Right;
        //
        // var currentLeft = (IElement)rootLeftValue;
        // while (currentLeft!.Parent is not null)
        // {
        //     Assert.NotEqual(currentLeft.Parent!.Left, currentLeft.Parent);
        //
        //     _op.WriteLine($"consider {currentLeft.Parent?.Left}");
        //     if (currentLeft.Parent?.Left is RegularNumber lrn)
        //     {
        //         _op.WriteLine($"found left {lrn}");
        //         break;
        //     }
        //     currentLeft = currentLeft.Parent?.Left;
        // }
    }

    private void Explode(IList<Pair> history)
    {
        var exploding = history.Last();

        var left = default(RegularNumber?);
        var right = default(RegularNumber?);

        var index = history.Count - 2;

        Pair? popped;
        while ((left is null || right is null) && (popped = history.ElementAtOrDefault(index)) is not null)
        {
            index -= 1;

            if (left is null && popped.Left is RegularNumber rnl)
            {
                left = rnl;
                popped.Replace(popped.Left, rnl.Value + ((RegularNumber)exploding.Left).Value);
            }

            if (right is null && popped.Right is RegularNumber rnr)
            {
                right = rnr;
                popped.Replace(popped.Right, rnr.Value + ((RegularNumber)exploding.Right).Value);
            }
        }

        history[^2].Replace(history[^1], 0);
    }

    private class Parser
    {
        private readonly char[] input;
        private int index;

        public Parser(string input) => this.input = input.ToCharArray();

        private char C => input[index];
        private bool IsSeparator => C == ',';
        private bool IsDigit => char.IsDigit(C);
        private bool IsOpener => C == '[';
        private bool IsCloser => C == ']';

        private Pair ParsePair()
        {
            if (!IsOpener) { throw new(); }
            index += 1;

            var left = ParseElement();

            if (!IsSeparator) { throw new(); }
            index += 1;

            var right = ParseElement();

            if (!IsCloser) { throw new(); }
            index += 1;

            return new Pair(left, right);
        }

        private IElement ParseElement()
        {
            if (IsOpener) { return ParsePair(); }
            if (IsDigit) { return ParseRegularNumber(); }
            throw new();
        }

        private RegularNumber ParseRegularNumber()
        {
            var n = 0;
            while (IsDigit)
            {
                n = n * 10 + (C - 48);
                index += 1;
            }
            if (C != ',' && C != ']') { throw new(); }
            return new(n);
        }

        public Pair Parse() => ParsePair();
    }

    private readonly record struct Token(string Body, TokenKind TokenKind)
    {
        public Token(char body, TokenKind tokenKind) : this(body.ToString(), tokenKind) { }
    }

    private enum TokenKind
    {
        Separator,
        Number,
        Opener,
        Closer,
    }

    private class Tokenizer
    {
        private readonly char[] input;
        private int index;

        public Tokenizer(string input) => this.input = input.ToCharArray();

        private char C => input[index];
        private bool IsSeparator => C == ',';
        private bool IsDigit => char.IsDigit(C);
        private bool IsOpener => C == '[';
        private bool IsCloser => C == ']';

        private TokenKind? Kind
        {
            get
            {
                if (IsSeparator)
                {
                    return TokenKind.Separator;
                }
                if (IsDigit)
                {
                    return TokenKind.Number;
                }
                if (IsOpener)
                {
                    return TokenKind.Opener;
                }
                if (IsCloser)
                {
                    return TokenKind.Closer;
                }
                return null;
            }
        }

        public IList<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (index != input.Length)
            {
                if (IsSeparator)
                {
                    tokens.Add(new(C, TokenKind.Separator));
                    index += 1;
                }
                else if (IsDigit)
                {
                    var stringBuilder = new StringBuilder();
                    while (IsDigit)
                    {
                        stringBuilder.Append(C);
                        index += 1;
                    }
                    tokens.Add(new(stringBuilder.ToString(), TokenKind.Number));
                }
                else if (IsOpener)
                {
                    tokens.Add(new(C, TokenKind.Opener));
                    index += 1;
                }
                else if (IsCloser)
                {
                    tokens.Add(new(C, TokenKind.Closer));
                    index += 1;
                }
            }
            return tokens;
        }

        private Pair ParsePair()
        {
            if (!IsOpener) { throw new(); }
            index += 1;

            var left = ParseElement();

            if (!IsSeparator) { throw new(); }
            index += 1;

            var right = ParseElement();

            if (!IsCloser) { throw new(); }
            index += 1;

            return new Pair(left, right);
        }

        private IElement ParseElement()
        {
            if (IsOpener) { return ParsePair(); }
            if (IsDigit) { return ParseRegularNumber(); }
            throw new();
        }

        private RegularNumber ParseRegularNumber()
        {
            var n = 0;
            while (IsDigit)
            {
                n = n * 10 + (C - 48);
                index += 1;
            }
            if (C != ',' && C != ']') { throw new(); }
            return new(n);
        }

        public Pair Parse() => ParsePair();
    }

    private record Pair : IElement
    {
        public Pair(IElement left, IElement right)
        {
            Left = left;
            Right = right;
            Left.Parent = this;
            Right.Parent = this;
        }

        public IElement Left { get; private set; }
        public IElement Right { get; private set; }

        public Pair? Parent { get; set; } = null;

        public override string ToString() => $"[{Left},{Right}]";

        public void Replace(IElement element, int regularNumberValue)
        {
            var regularNumber = new RegularNumber(regularNumberValue);
            if (Left == element) { Left = regularNumber; }
            else if (Right == element) { Right = regularNumber; }
        }

        public void Deconstruct(out IElement left, out IElement right)
        {
            left = Left;
            right = Right;
        }
    }

    private interface IElement
    {
        Pair? Parent { get; set; }
    }

    private record RegularNumber(int Value) : IElement
    {
        public Pair? Parent { get; set; } = null;
        public override string ToString() => Value.ToString();
    }
}