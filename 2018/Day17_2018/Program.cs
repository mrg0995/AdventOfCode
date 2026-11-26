using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var clay = new HashSet<(int X, int Y)>();
foreach (var line in File.ReadAllLines("input.txt").Where(l => !string.IsNullOrWhiteSpace(l)))
{
    var nums = Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray();
    if (line.StartsWith("x="))
        for (int y = nums[1]; y <= nums[2]; y++) clay.Add((nums[0], y));
    else
        for (int x = nums[1]; x <= nums[2]; x++) clay.Add((x, nums[0]));
}

int minY = clay.Min(p => p.Y);
int maxY = clay.Max(p => p.Y);
var settled = new HashSet<(int X, int Y)>();
var flowing = new HashSet<(int X, int Y)>();

bool Flow(int x, int y)
{
    if (y > maxY) return false;
    if (clay.Contains((x, y)) || settled.Contains((x, y))) return true;
    if (flowing.Contains((x, y))) return false;

    flowing.Add((x, y));
    if (!Flow(x, y + 1)) return false;

    int lx = x;
    bool leftBounded = false;
    while (true)
    {
        if (clay.Contains((lx - 1, y))) { leftBounded = true; break; }
        flowing.Add((lx - 1, y));
        if (!Flow(lx - 1, y + 1)) break;
        lx--;
    }

    int rx = x;
    bool rightBounded = false;
    while (true)
    {
        if (clay.Contains((rx + 1, y))) { rightBounded = true; break; }
        flowing.Add((rx + 1, y));
        if (!Flow(rx + 1, y + 1)) break;
        rx++;
    }

    if (leftBounded && rightBounded)
    {
        for (int wx = lx; wx <= rx; wx++)
        {
            flowing.Remove((wx, y));
            settled.Add((wx, y));
        }
        return true;
    }

    return false;
}

Flow(500, minY);
var sRange = settled.Where(p => p.Y >= minY && p.Y <= maxY).ToHashSet();
var fRange = flowing.Where(p => p.Y >= minY && p.Y <= maxY).ToHashSet();
var allWater = new HashSet<(int X, int Y)>(sRange);
allWater.UnionWith(fRange);

Console.WriteLine($"Day 17 -> Part 1: {allWater.Count} | Part 2: {sRange.Count}");