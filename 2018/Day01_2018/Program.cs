using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var deltas = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(int.Parse)
    .ToArray();

int part1 = deltas.Sum();
int freq = 0;
var seen = new HashSet<int> { 0 };
int? part2 = null;

while (part2 is null)
{
    foreach (int d in deltas)
    {
        freq += d;
        if (!seen.Add(freq)) { part2 = freq; break; }
    }
}

Console.WriteLine($"Day 1 -> Part 1: {part1} | Part 2: {part2}");
