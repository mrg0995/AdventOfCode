using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .OrderBy(l => l)
    .ToArray();

var sleep = new Dictionary<int, int[]>();
int guard = 0, start = 0;

foreach (var line in lines)
{
    int minute = int.Parse(line.Substring(15, 2));
    if (line.Contains("Guard #"))
    {
        guard = int.Parse(Regex.Match(line, @"#(\d+)").Groups[1].Value);
        sleep.TryAdd(guard, new int[60]);
    }
    else if (line.Contains("falls asleep"))
    {
        start = minute;
    }
    else if (line.Contains("wakes up"))
    {
        for (int m = start; m < minute; m++) sleep[guard][m]++;
    }
}

var g1 = sleep.MaxBy(kv => kv.Value.Sum());
int m1 = Array.IndexOf(g1.Value, g1.Value.Max());
int part1 = g1.Key * m1;

var g2 = sleep.MaxBy(kv => kv.Value.Max());
int m2 = Array.IndexOf(g2.Value, g2.Value.Max());
int part2 = g2.Key * m2;

Console.WriteLine($"Day 4 -> Part 1: {part1} | Part 2: {part2}");
