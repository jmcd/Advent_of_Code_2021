namespace AoC;

using System.IO;
using System.Threading.Tasks;

internal static class Input
{
    public static async Task<string[]> ReadAllLinesAsync(string filename) => await File.ReadAllLinesAsync(Path.Combine("input", filename));
}