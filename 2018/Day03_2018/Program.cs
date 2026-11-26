using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var rx = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");
var claims = File.ReadAllLines("input.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Select(l => rx.Match(l).Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray())
    .Select(a => (Id: a[0], X: a[1], Y: a[2], W: a[3], H: a[4]))
    .ToArray();

var fabric = new Dictionary<(int X, int Y), int>();
foreach (var c in claims)
    for (int dx = 0; dx < c.W; dx++)
        for (int dy = 0; dy < c.H; dy++)
        {
            var key = (c.X + dx, c.Y + dy);
            fabric[key] = fabric.GetValueOrDefault(key) + 1;
        }

int part1 = fabric.Values.Count(v => v >= 2);
int part2 = claims.First(c =>
    Enumerable.Range(0, c.W).All(dx =>
        Enumerable.Range(0, c.H).All(dy => fabric[(c.X + dx, c.Y + dy)] == 1))).Id;

Console.WriteLine($"Day 3 -> Part 1: {part1} | Part 2: {part2}");