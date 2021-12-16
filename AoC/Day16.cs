namespace AoC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day16
{
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
        var hexInput = await GetHexInput(input, filename);
        var rootPacket = new PacketReader(hexInput).ReadRootPacket();

        IEnumerable<int> GetAllVersions(Packet packet)
        {
            void Recurse(Packet packet, ICollection<int> versions)
            {
                versions.Add(packet.Version);
                foreach (var subPacket in packet.SubPackets)
                {
                    Recurse(subPacket, versions);
                }
            }

            var versions = new List<int>();
            Recurse(packet, versions);
            return versions;
        }

        var versionSum = GetAllVersions(rootPacket).Sum();

        Assert.Equal(expectation, versionSum);
    }

    [Theory]
    [InlineData("C200B40A82", null, 3)]
    [InlineData("04005AC33890", null, 54)]
    [InlineData("880086C3E88112", null, 7)]
    [InlineData("CE00C43D881120", null, 9)]
    [InlineData("D8005AC2A8F0", null, 1)]
    [InlineData("F600BC2D8F", null, 0)]
    [InlineData("9C005AC2F8F0", null, 0)]
    [InlineData("9C0141080250320F1802104A08", null, 1)]
    [InlineData(null, "day16.txt", 7936430475134)]
    public async Task Part2(string? input, string? filename, long expectation)
    {
        var hexInput = await GetHexInput(input, filename);
        var rootPacket = new PacketReader(hexInput).ReadRootPacket();
        var value = rootPacket.CalculateValue();
        Assert.Equal(expectation, value);
    }

    private static async Task<string> GetHexInput(string? input, string? filename)
    {
        if (input is not null) { return input; }
        if (filename is not null) { return await Input.ReadSingleLineAsync(filename); }
        throw new();
    }

    private class PacketReader
    {
        private readonly InputBits bits;

        public PacketReader(string hexInput) => bits = new(hexInput);

        public Packet ReadRootPacket()
        {
            var rootPacket = ReadPacket();

            if (bits.Remaining > 0)
            {
                var read = bits.Read(bits.Remaining);
                if (read != 0) { throw new(); }
            }
            return rootPacket;
        }

        private Packet ReadPacket()
        {
            var version = bits.Read(3);
            var typeId = (TypeId)bits.Read(3);
            return typeId == TypeId.Literal ? ReadLiteral(version) : ReadOperator(version, typeId);
        }

        private Packet ReadOperator(int version, TypeId typeID)
        {
            var subPackets = ReadSubPackets();

            return Packet.Operator(version, typeID, subPackets);
        }

        private Packet ReadLiteral(int version)
        {
            int read;
            var literalValue = 0L;
            do
            {
                read = bits.Read(5);
                long val = read & 0b01111;
                literalValue = (literalValue << 4) | val;
            } while (read >> 4 == 1);
            return Packet.Literal(version, literalValue);
        }

        private IEnumerable<Packet> ReadSubPackets()
        {
            var lengthTypeId = bits.Read(1);
            switch (lengthTypeId)
            {
                case 0: return ReadSubPacketsOfSpecifiedLength();
                case 1: return ReadSpecifiedNumberOfSubPackets();
                default: throw new();
            }
        }

        private IEnumerable<Packet> ReadSpecifiedNumberOfSubPackets()
        {
            var numberOfSubPacketsImmediatelyContainedByThisPacket = bits.Read(11);
            for (var i = 0; i < numberOfSubPacketsImmediatelyContainedByThisPacket; i++)
            {
                yield return ReadPacket();
            }
        }

        private IEnumerable<Packet> ReadSubPacketsOfSpecifiedLength()
        {
            var totalLengthInBits = bits.Read(15);
            var start = bits.Position;
            while (bits.Position - start != totalLengthInBits)
            {
                yield return ReadPacket();
            }
        }
    }

    private enum TypeId
    {
        Sum,
        Product,
        Minimum,
        Maximum,
        Literal,
        GreaterThan,
        LessThan,
        EqualTo,
    }

    private record Packet
    {
        private Packet(int version, TypeId typeId, Packet[] subPackets, long? literalValue)
        {
            Version = version;
            TypeId = typeId;
            SubPackets = subPackets;
            LiteralValue = literalValue;
        }

        public int Version { get; }
        private TypeId TypeId { get; }
        public Packet[] SubPackets { get; }
        private long? LiteralValue { get; }

        public static Packet Literal(int version, long value) => new(version, TypeId.Literal, Array.Empty<Packet>(), value);
        public static Packet Operator(int version, TypeId typeId, IEnumerable<Packet> subPackets) => new(version, typeId, subPackets.ToArray(), default);

        public long CalculateValue()
        {
            var subPacketValues = SubPackets.Select(sp => sp.CalculateValue()).ToArray();
            switch (TypeId)
            {
                case TypeId.Sum:
                    return subPacketValues.Sum();
                case TypeId.Product:
                    return subPacketValues.Aggregate(1L, (acc, spv) => acc * spv);
                case TypeId.Minimum:
                    return subPacketValues.Min();
                case TypeId.Maximum:
                    return subPacketValues.Max();
                case TypeId.Literal:
                    return LiteralValue!.Value;
                case TypeId.GreaterThan:
                    return subPacketValues[0] > subPacketValues[1] ? 1 : 0;
                case TypeId.LessThan:
                    return subPacketValues[0] < subPacketValues[1] ? 1 : 0;
                case TypeId.EqualTo:
                    var value = subPacketValues[0] == subPacketValues[1] ? 1 : 0;
                    return value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

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