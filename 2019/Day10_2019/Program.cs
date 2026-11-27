using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day10
{
    static double ClockwiseAngle(int dx, int dy)
    {
        double ang = Math.Atan2(dx, -dy);
        return ang >= 0.0 ? ang : ang + 2.0 * Math.PI;
    }

    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var asteroids = new List<(int x, int y)>();
        for (int y = 0; y < lines.Length; y++)
        {
            string row = lines[y].TrimEnd('\r', '\n');
            if (string.IsNullOrEmpty(row)) continue;
            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] == '#') asteroids.Add((x, y));
            }
        }

        (int x, int y) bestPos = (0, 0);
        int bestCount = -1;

        foreach (var a in asteroids)
        {
            var angles = new HashSet<double>();
            foreach (var b in asteroids)
            {
                if (a == b) continue;
                angles.Add(ClockwiseAngle(b.x - a.x, b.y - a.y));
            }

            if (angles.Count > bestCount)
            {
                bestCount = angles.Count;
                bestPos = a;
            }
        }

        var byAngle = new SortedDictionary<double, Queue<(int x, int y)>>();
        var grouped = new Dictionary<double, List<(double dist, int x, int y)>>();

        foreach (var b in asteroids)
        {
            if (b == bestPos) continue;
            int dx = b.x - bestPos.x;
            int dy = b.y - bestPos.y;
            double ang = ClockwiseAngle(dx, dy);
            double dist = Math.Sqrt((double)dx * dx + (double)dy * dy);

            if (!grouped.TryGetValue(ang, out var list))
            {
                list = new List<(double dist, int x, int y)>();
                grouped[ang] = list;
            }
            list.Add((dist, b.x, b.y));
        }

        foreach (var (ang, list) in grouped)
        {
            list.Sort((u, v) => u.dist.CompareTo(v.dist));
            var q = new Queue<(int x, int y)>(list.Select(t => (t.x, t.y)));
            byAngle[ang] = q;
        }

        int vaporized = 0;
        (int x, int y) target = (0, 0);
        var queues = byAngle.Values.ToList();

        while (vaporized < 200 && queues.Any(q => q.Count > 0))
        {
            foreach (var q in queues)
            {
                if (q.Count > 0)
                {
                    target = q.Dequeue();
                    vaporized++;
                    if (vaporized == 200) break;
                }
            }
        }

        int p2 = target.x * 100 + target.y;
        return (bestCount, p2);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day10_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 10 - Part 1: {p1}, Part 2: {p2}");
    }
}