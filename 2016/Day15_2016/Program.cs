
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

record Disc(int Number, int Positions, int StartPos);

class Program
{
    static readonly Regex LineRegex = new(
        @"^Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+)\.$",
        RegexOptions.Compiled
    );

    static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long Lcm(long a, long b) => (a / Gcd(a, b)) * b;

    static List<Disc> ParseDiscs(string[] lines)
    {
        var discs = new List<Disc>();
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var match = LineRegex.Match(line);
            if (!match.Success)
                throw new FormatException($"Línea no reconocida: {line}");

            int num = int.Parse(match.Groups[1].Value);
            int positions = int.Parse(match.Groups[2].Value);
            int startPos = int.Parse(match.Groups[3].Value);
            discs.Add(new Disc(num, positions, startPos));
        }
        return discs;
    }

    static long FindFirstTime(IEnumerable<Disc> discs)
    {
        long t = 0;
        long step = 1;

        foreach (var disc in discs)
        {
            while ((disc.StartPos + t + disc.Number) % disc.Positions != 0)
            {
                t += step;
            }
            step = Lcm(step, disc.Positions);
        }

        return t;
    }

    static void Main()
    {
        string[] lines = File.ReadAllLines("Your_Route");
        var discs = ParseDiscs(lines);

        long part1 = FindFirstTime(discs);

        var discsPart2 = new List<Disc>(discs)
        {
            new Disc(discs.Count + 1, 11, 0)
        };
        long part2 = FindFirstTime(discsPart2);

        Console.WriteLine($"Parte 1: {part1}");
        Console.WriteLine($"Parte 2: {part2}");
    }
}