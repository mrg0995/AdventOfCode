using System;
using System.Collections.Generic;
using System.IO;

static class Day20
{
    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var nonEmpty = new List<string>();
        foreach (var l in lines)
        {
            var s = l.TrimEnd('\r', '\n');
            if (!string.IsNullOrEmpty(s)) nonEmpty.Add(s);
        }

        int h = nonEmpty.Count;
        int w = 0;
        foreach (var r in nonEmpty) if (r.Length > w) w = r.Length;

        var grid = new char[h, w];
        var openTiles = new HashSet<(int x, int y)>();
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                char ch = x < nonEmpty[y].Length ? nonEmpty[y][x] : ' ';
                grid[y, x] = ch;
                if (ch == '.') openTiles.Add((x, y));
            }
        }

        var rawPortals = new Dictionary<string, List<(int x, int y)>>();
        void AddPortal(string code, (int x, int y) pt)
        {
            if (!rawPortals.TryGetValue(code, out var list))
            {
                list = new List<(int x, int y)>();
                rawPortals[code] = list;
            }
            list.Add(pt);
        }

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                char ch = grid[y, x];
                if (ch is < 'A' or > 'Z') continue;

                if (x + 1 < w && grid[y, x + 1] is >= 'A' and <= 'Z')
                {
                    string code = $"{ch}{grid[y, x + 1]}";
                    if (x - 1 >= 0 && grid[y, x - 1] == '.') AddPortal(code, (x - 1, y));
                    else if (x + 2 < w && grid[y, x + 2] == '.') AddPortal(code, (x + 2, y));
                }

                if (y + 1 < h && grid[y + 1, x] is >= 'A' and <= 'Z')
                {
                    string code = $"{ch}{grid[y + 1, x]}";
                    if (y - 1 >= 0 && grid[y - 1, x] == '.') AddPortal(code, (x, y - 1));
                    else if (y + 2 < h && grid[y + 2, x] == '.') AddPortal(code, (x, y + 2));
                }
            }
        }

        bool IsOuter((int x, int y) pt) => pt.x <= 2 || pt.x >= w - 3 || pt.y <= 2 || pt.y >= h - 3;

        var warp = new Dictionary<(int x, int y), ((int x, int y) dest, int dLvl)>();
        foreach (var (_, pts) in rawPortals)
        {
            if (pts.Count == 2)
            {
                var a = pts[0];
                var b = pts[1];
                warp[a] = (b, IsOuter(a) ? -1 : 1);
                warp[b] = (a, IsOuter(b) ? -1 : 1);
            }
        }

        var start = rawPortals["AA"][0];
        var goal = rawPortals["ZZ"][0];
        (int dx, int dy)[] dirs = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        var q1 = new Queue<((int x, int y) pos, int dist)>();
        var seen1 = new HashSet<(int x, int y)> { start };
        q1.Enqueue((start, 0));
        int p1 = -1;

        while (q1.Count > 0)
        {
            var (pos, dist) = q1.Dequeue();
            if (pos == goal) { p1 = dist; break; }

            var nexts = new List<(int x, int y)>();
            foreach (var (dx, dy) in dirs) nexts.Add((pos.x + dx, pos.y + dy));
            if (warp.TryGetValue(pos, out var wInfo)) nexts.Add(wInfo.dest);

            foreach (var nxt in nexts)
            {
                if (openTiles.Contains(nxt) && seen1.Add(nxt))
                {
                    q1.Enqueue((nxt, dist + 1));
                }
            }
        }

        var q2 = new Queue<((int x, int y) pos, int level, int dist)>();
        var seen2 = new HashSet<((int x, int y) pos, int level)> { (start, 0) };
        q2.Enqueue((start, 0, 0));
        int p2 = -1;

        while (q2.Count > 0)
        {
            var (pos, level, dist) = q2.Dequeue();
            if (pos == goal && level == 0) { p2 = dist; break; }

            foreach (var (dx, dy) in dirs)
            {
                var nxt = (pos.x + dx, pos.y + dy);
                if (openTiles.Contains(nxt) && seen2.Add((nxt, level)))
                {
                    q2.Enqueue((nxt, level, dist + 1));
                }
            }

            if (warp.TryGetValue(pos, out var wInfo))
            {
                int nLvl = level + wInfo.dLvl;
                if (nLvl >= 0 && seen2.Add((wInfo.dest, nLvl)))
                {
                    q2.Enqueue((wInfo.dest, nLvl, dist + 1));
                }
            }
        }

        return (p1, p2);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day20_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 20 - Part 1: {p1}, Part 2: {p2}");
    }
}