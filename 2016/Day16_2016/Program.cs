using System.Diagnostics;
using System.Text;

const string puzzleInput = "01111001100111011";

const int diskLengthPart1 = 272;
const int diskLengthPart2 = 35_651_584;

var sw = Stopwatch.StartNew();

string part1 = Solve(puzzleInput, diskLengthPart1);
string part2 = Solve(puzzleInput, diskLengthPart2);

sw.Stop();

Console.WriteLine($"Parte 1 (longitud {diskLengthPart1}): {part1}");
Console.WriteLine($"Parte 2 (longitud {diskLengthPart2}): {part2}");
Console.WriteLine($"Tiempo total: {sw.ElapsedMilliseconds} ms");

static string Solve(string initialState, int targetLength)
{
    bool[] data = GenerateDragonData(initialState, targetLength);
    bool[] checksum = ComputeChecksum(data, targetLength);

    var sb = new StringBuilder(checksum.Length);
    foreach (bool bit in checksum)
    {
        sb.Append(bit ? '1' : '0');
    }

    return sb.ToString();
}

static bool[] GenerateDragonData(string initialState, int targetLength)
{
    var bits = new List<bool>(targetLength * 2);
    foreach (char c in initialState)
    {
        bits.Add(c == '1');
    }

    while (bits.Count < targetLength)
    {
        int currentCount = bits.Count;
        bits.Add(false);

        for (int i = currentCount - 1; i >= 0; i--)
        {
            bits.Add(!bits[i]);
        }
    }

    var result = new bool[targetLength];
    bits.CopyTo(0, result, 0, targetLength);
    return result;
}

static bool[] ComputeChecksum(bool[] current, int length)
{
    while (true)
    {
        int nextLength = length / 2;
        var next = new bool[nextLength];

        for (int i = 0; i < nextLength; i++)
        {
            next[i] = current[2 * i] == current[2 * i + 1];
        }

        current = next;
        length = nextLength;

        if (length % 2 == 1)
        {
            return current;
        }
    }
}
