namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Day16
{
    private ITestOutputHelper _op;
    public Day16(ITestOutputHelper op) => _op = op;

    [Theory]
    [InlineData("D2FE28", null, 0b110)]
    [InlineData("38006F45291200", null, 0b001 + 0b110 + 0b10)]
    [InlineData("EE00D40C823060", null, 0b111 + 0b010 + 0b100 + 0b001)]
    [InlineData("8A004A801A8002F478", null, 16)]
    [InlineData("620080001611562C8802118E34", null, 12)]
    [InlineData("C0015000016115A2E0802F182340", null, 23)]
    [InlineData("A0016C880162017C3686B18A3D4780", null, 31)]
    [InlineData(null, "day16.txt", 989)]
    public async Task Part1(string? input, string? filename, int expectation)
    {
        string nullSafeInput;
        if (input is not null) { nullSafeInput = input; }
        else if (filename is not null) { nullSafeInput = await Input.ReadSingleLineAsync(filename); }
        else { throw new(); }

        IEnumerable<int> GetAllVersions(IPacket packet)
        {
            void Recurse(IPacket packet, ICollection<int> versions)
            {
                versions.Add(packet.Version);
                if (packet is not Operator @operator) { return; }
                foreach (var subPacket in @operator.SubPackets)
                {
                    Recurse(subPacket, versions);
                }
            }

            var versions = new List<int>();
            Recurse(packet, versions);
            return versions;
        }

        var bits = new InputBits(nullSafeInput);

        var rootPacket = ReadPacket(bits);

        if (bits.Remaining > 0)
        {
            _op.WriteLine($"checking final {bits.Remaining} are zero");
            var read = bits.Read(bits.Remaining);
            Assert.Equal(0, read);
        }

        var versionSum = GetAllVersions(rootPacket).Sum();

        Assert.Equal(expectation, versionSum);
    }

    private IPacket ReadPacket(InputBits bits)
    {
        var version = bits.Read(3);
        var typeID = bits.Read(3);

        _op.WriteLine($"version {version}");

        if (typeID == 4) // literal value
        {
            _op.WriteLine("reading literal...");

            var read = 0;
            var literal = 0;
            do
            {
                read = bits.Read(5);
                var val = read & 0b01111;
                literal = (literal << 4) | val;
            } while (read >> 4 == 1);

            _op.WriteLine($"read {literal}");

            return new Literal(version);
        }

        _op.WriteLine("reading operator...");

        var lengthTypeID = bits.Read(1);

        _op.WriteLine($"read lengthTypeID: {lengthTypeID}");

        switch (lengthTypeID)
        {
            // If the length type ID is 0, then the next 15 bits are a number that represents the total length in bits of the sub-packets contained by this packet.
            case 0:
            {
                var totalLengthInBits = bits.Read(15);

                _op.WriteLine($"read totalLengthInBits: {totalLengthInBits}");

                var start = bits.Position;

                var subPackets = new List<IPacket>();
                while (bits.Position - start != totalLengthInBits)
                {
                    subPackets.Add(ReadPacket(bits));
                    _op.WriteLine($"did read {bits.Position - start} bits");
                }
                _op.WriteLine("done reading sub-packets");

                return new Operator(version, subPackets.ToArray());
            }
            // If the length type ID is 1, then the next 11 bits are a number that represents the number of sub-packets immediately contained by this packet.
            case 1:
            {
                var numberOfSubPacketsImmediatelyContainedByThisPacket = bits.Read(11);

                var subPackets = new List<IPacket>();
                for (var i = 0; i < numberOfSubPacketsImmediatelyContainedByThisPacket; i++)
                {
                    subPackets.Add(ReadPacket(bits));
                }
                return new Operator(version, subPackets.ToArray());
            }
            default:
                throw new();
        }
    }

    private interface IPacket
    {
        int Version { get; }
    }

    private record Literal(int Version) : IPacket { }

    private record Operator(int Version, IPacket[] SubPackets) : IPacket;

    private class InputBits
    {
        private static readonly int[] Table;

        static InputBits()
        {
            Table = new int['F' + 1];
            Table['0'] = 0b0000;
            Table['1'] = 0b0001;
            Table['2'] = 0b0010;
            Table['3'] = 0b0011;
            Table['4'] = 0b0100;
            Table['5'] = 0b0101;
            Table['6'] = 0b0110;
            Table['7'] = 0b0111;
            Table['8'] = 0b1000;
            Table['9'] = 0b1001;
            Table['A'] = 0b1010;
            Table['B'] = 0b1011;
            Table['C'] = 0b1100;
            Table['D'] = 0b1101;
            Table['E'] = 0b1110;
            Table['F'] = 0b1111;
        }

        public InputBits(string input)
        {
            Input = input;
            Length = input.Length * 4;
            Position = 0;
        }

        private string Input { get; }
        public int Position { get; private set; }
        private int Length { get; }

        public int Remaining => Length - Position;

        public int Read(int numberOfBits)
        {
            if (numberOfBits + Position > Length) { throw new InvalidOperationException(); }

            var mod = Position % 4;

            var numberOfBitsUsed = numberOfBits + mod;

            var numberOfCharactersUsed = numberOfBitsUsed / 4 + (numberOfBitsUsed % 4 > 0 ? 1 : 0);

            var result = 0;
            for (var numberOfCharactersRead = 0; numberOfCharactersRead < numberOfCharactersUsed; numberOfCharactersRead++)
            {
                result <<= 4;
                result |= Table[Input[Position / 4 + numberOfCharactersRead]];
                if (numberOfCharactersRead == 0)
                {
                    result &= 0b1111 >> mod;
                }
            }

            result >>= numberOfCharactersUsed * 4 - numberOfBits - mod;
            Position += numberOfBits;
            return result;
        }
    }
}