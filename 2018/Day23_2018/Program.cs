using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var bots = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(l => Regex.Matches(l, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray())
    .Select(a => (X: a[0], Y: a[1], Z: a[2], R: a[3]))
    .ToArray();

var strongest = bots.MaxBy(b => b.R);
int part1 = bots.Count(b => Math.Abs(b.X - strongest.X) + Math.Abs(b.Y - strongest.Y) + Math.Abs(b.Z - strongest.Z) <= strongest.R);

int span = new[]
{
    bots.Max(b => b.X) - bots.Min(b => b.X),
    bots.Max(b => b.Y) - bots.Min(b => b.Y),
    bots.Max(b => b.Z) - bots.Min(b => b.Z)
}.Max();

int step = 1;
while (step < span) step *= 2;

var bestPos = (X: 0, Y: 0, Z: 0);
while (step >= 1)
{
    int bestCount = -1;
    long bestOriginDist = long.MaxValue;
    var (cx, cy, cz) = bestPos;

    for (int x = cx - step * 2; x <= cx + step * 2; x += step)
    {
        for (int y = cy - step * 2; y <= cy + step * 2; y += step)
        {
            for (int z = cz - step * 2; z <= cz + step * 2; z += step)
            {
                int cnt = 0;
                int margin = (step - 1) * 3;
                foreach (var b in bots)
                {
                    long d = Math.Abs((long)x - b.X) + Math.Abs((long)y - b.Y) + Math.Abs((long)z - b.Z);
                    if (d <= (long)b.R + margin) cnt++;
                }

                long originDist = Math.Abs((long)x) + Math.Abs((long)y) + Math.Abs((long)z);
                if (cnt > bestCount || (cnt == bestCount && originDist < bestOriginDist))
                {
                    bestCount = cnt;
                    bestOriginDist = originDist;
                    bestPos = (x, y, z);
                }
            }
        }
    }
    step /= 2;
}

long part2 = Math.Abs((long)bestPos.X) + Math.Abs((long)bestPos.Y) + Math.Abs((long)bestPos.Z);
Console.WriteLine($"Day 23 -> Part 1: {part1} | Part 2: {part2}");
