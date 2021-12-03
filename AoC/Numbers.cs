namespace AoC;

internal static class Numbers
{
    internal static int GetBit(this int number, int bitIndex) => (number >> bitIndex) & 1;
}