using System.Globalization;

const long MaxIp = 4294967295L;
string inputPath = args.Length > 0 ? args[0] : "Your_Route";

List<(long Start, long End)> ranges = File.ReadAllLines(inputPath)
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(ParseRange)
    .OrderBy(r => r.Start)
    .ThenBy(r => r.End)
    .ToList();

long part1 = SolvePart1(ranges);
long part2 = SolvePart2(ranges, MaxIp);

Console.WriteLine($"Parte 1 (IP más baja permitida): {part1}");
Console.WriteLine($"Parte 2 (Total de IPs permitidas): {part2}");

static (long Start, long End) ParseRange(string line)
{
    string[] parts = line.Trim().Split('-', StringSplitOptions.TrimEntries);
    if (parts.Length != 2 ||
        !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out long start) ||
        !long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out long end))
    {
        throw new FormatException($"Línea con formato inválido: '{line}'");
    }

    return (start, end);
}

static long SolvePart1(List<(long Start, long End)> sortedRanges)
{
    long candidate = 0;

    foreach (var (start, end) in sortedRanges)
    {
        if (start > candidate)
        {
            return candidate;
        }

        if (end >= candidate)
        {
            candidate = end + 1;
        }
    }

    return candidate;
}

static long SolvePart2(List<(long Start, long End)> sortedRanges, long maxIp)
{
    if (sortedRanges.Count == 0)
    {
        return maxIp + 1;
    }

    long allowedCount = 0;
    long currentEnd = sortedRanges[0].End;

    if (sortedRanges[0].Start > 0)
    {
        allowedCount += sortedRanges[0].Start;
    }

    for (int i = 1; i < sortedRanges.Count; i++)
    {
        var (start, end) = sortedRanges[i];

        if (start > currentEnd + 1)
        {
            allowedCount += start - currentEnd - 1;
        }

        if (end > currentEnd)
        {
            currentEnd = end;
        }
    }

    if (currentEnd < maxIp)
    {
        allowedCount += maxIp - currentEnd;
    }

    return allowedCount;
}