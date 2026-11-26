using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var rx = new Regex(@"position=<\s*(-?\d+),\s*(-?\d+)> velocity=<\s*(-?\d+),\s*(-?\d+)>");
var stars = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(l => rx.Match(l).Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray())
    .Select(a => (X: a[0], Y: a[1], Vx: a[2], Vy: a[3]))
    .ToArray();

int t = 0;
long prevArea = long.MaxValue;

while (true)
{
    var pos = stars.Select(s => (X: (long)s.X + (long)s.Vx * t, Y: (long)s.Y + (long)s.Vy * t)).ToArray();
    long area = (pos.Max(p => p.X) - pos.Min(p => p.X)) * (pos.Max(p => p.Y) - pos.Min(p => p.Y));
    if (area > prevArea) { t--; break; }
    prevArea = area;
    t++;
}

var finalPts = stars.Select(s => (X: s.X + s.Vx * t, Y: s.Y + s.Vy * t)).ToHashSet();
int minX = finalPts.Min(p => p.X), maxX = finalPts.Max(p => p.X);
int minY = finalPts.Min(p => p.Y), maxY = finalPts.Max(p => p.Y);

Console.WriteLine($"Day 10 -> Part 2: {t}s\nPart 1:");
for (int y = minY; y <= maxY; y++)
{
    for (int x = minX; x <= maxX; x++)
        Console.Write(finalPts.Contains((x, y)) ? '#' : '.');
    Console.WriteLine();
}
