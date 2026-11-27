using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day03
{
    static readonly Dictionary<char, (int dx, int dy)> Dirs = new()
    {
        ['R'] = (1, 0),
        ['L'] = (-1, 0),
        ['U'] = (0, 1),
        ['D'] = (0, -1),
    };

    static Dictionary<(int x, int y), int> TraceWire(string path)
    {
        int x = 0, y = 0, steps = 0;
        var visited = new Dictionary<(int x, int y), int>();

        foreach (var rawToken in path.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var (dx, dy) = Dirs[rawToken[0]];
            int dist = int.Parse(rawToken[1..]);

            for (int i = 0; i < dist; i++)
            {
                x += dx;
                y += dy;
                steps++;
                var pos = (x, y);
                visited.TryAdd(pos, steps);
            }
        }

        return visited;
    }

    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var nonEmpty = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        var w1 = TraceWire(nonEmpty[0]);
        var w2 = TraceWire(nonEmpty[1]);

        var intersections = w1.Keys.Intersect(w2.Keys).ToArray();
        int p1 = intersections.Min(pos => Math.Abs(pos.x) + Math.Abs(pos.y));
        int p2 = intersections.Min(pos => w1[pos] + w2[pos]);

        return (p1, p2);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day03_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 3 - Part 1: {p1}, Part 2: {p2}");
    }
}