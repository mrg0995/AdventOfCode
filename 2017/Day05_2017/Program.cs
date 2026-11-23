using System.Diagnostics;

string inputPath = "input.txt";

int[] baseInstructions = File.ReadAllLines(inputPath)
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(int.Parse)
    .ToArray();

var sw = Stopwatch.StartNew();
long part1Steps = Solve((int[])baseInstructions.Clone(), isPart2: false);
long t1 = sw.ElapsedMilliseconds;

sw.Restart();
long part2Steps = Solve((int[])baseInstructions.Clone(), isPart2: true);
long t2 = sw.ElapsedMilliseconds;

Console.WriteLine($"Parte 1: {part1Steps} pasos ({t1} ms)");
Console.WriteLine($"Parte 2: {part2Steps} pasos ({t2} ms)");

static long Solve(int[] jumps, bool isPart2)
{
    int index = 0;
    long steps = 0;
    int length = jumps.Length;

    while (index >= 0 && index < length)
    {
        int offset = jumps[index];
        jumps[index] += (isPart2 && offset >= 3) ? -1 : 1;
        index += offset;
        steps++;
    }

    return steps;
}