using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var pts = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(l => l.Split(", ").Select(int.Parse).ToArray())
    .Select(a => (X: a[0], Y: a[1]))
    .ToArray();

int minX = pts.Min(p => p.X), maxX = pts.Max(p => p.X);
int minY = pts.Min(p => p.Y), maxY = pts.Max(p => p.Y);

var areas = new int[pts.Length];
var infinite = new HashSet<int>();
int safeRegion = 0;

for (int x = minX; x <= maxX; x++)
{
    for (int y = minY; y <= maxY; y++)
    {
        var dists = pts.Select((p, i) => (Dist: Math.Abs(x - p.X) + Math.Abs(y - p.Y), Idx: i))
            .OrderBy(t => t.Dist)
            .ToArray();

        if (dists.Sum(t => t.Dist) < 10_000) safeRegion++;

        if (dists[0].Dist < dists[1].Dist)
        {
            int owner = dists[0].Idx;
            areas[owner]++;
            if (x == minX || x == maxX || y == minY || y == maxY)
                infinite.Add(owner);
        }
    }
}

int part1 = areas.Where((_, i) => !infinite.Contains(i)).Max();
Console.WriteLine($"Day 6 -> Part 1: {part1} | Part 2: {safeRegion}");
