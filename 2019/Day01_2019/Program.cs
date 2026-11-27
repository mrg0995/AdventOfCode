using System;
using System.IO;
using System.Linq;

static class Day01
{
    static int FuelPart1(int mass) => (mass / 3) - 2;

    static int FuelPart2(int mass)
    {
        int total = 0;
        int f = FuelPart1(mass);
        while (f > 0)
        {
            total += f;
            f = FuelPart1(f);
        }
        return total;
    }

    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var masses = lines
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => int.Parse(l.Trim()))
            .ToArray();

        return (masses.Sum(FuelPart1), masses.Sum(FuelPart2));
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day01_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 1 - Part 1: {p1}, Part 2: {p2}");
    }
}