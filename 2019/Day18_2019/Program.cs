using System;
using System.Collections.Generic;
using System.IO;

static class Day18
{
    readonly record struct State4(
        (int x, int y) r0,
        (int x, int y) r1,
        (int x, int y) r2,
        (int x, int y) r3,
        int mask);

    static int BfsSingle(string[] grid, (int x, int y) start, int allMask)
    {
        int h = grid.Length, w = grid[0].Length;
        var q = new Queue<((int x, int y) pos, int mask, int steps)>();
        var seen = new HashSet<((int x, int y) pos, int mask)>();

        q.Enqueue((start, 0, 0));
        seen.Add((start, 0));

        (int dx, int dy)[] dirs = { (-1, 0), (1, 0), (0, -1), (0, 1) };
        while (q.Count > 0)
        {
            var (pos, mask, steps) = q.Dequeue();
            if (mask == allMask) return steps;

            foreach (var (dx, dy) in dirs)
            {
                int nx = pos.x + dx, ny = pos.y + dy;
                if (ny < 0 || ny >= h || nx < 0 || nx >= w) continue;
                char ch = grid[ny][nx];
                if (ch == '#') continue;

                if (ch is >= 'A' and <= 'Z')
                {
                    int doorBit = 1 << (ch - 'A');
                    if ((mask & doorBit) == 0) continue;
                }

                int newMask = mask;
                if (ch is >= 'a' and <= 'z') newMask |= 1 << (ch - 'a');

                var nextPos = (nx, ny);
                if (seen.Add((nextPos, newMask)))
                {
                    q.Enqueue((nextPos, newMask, steps + 1));
                }
            }
        }
        return -1;
    }

    static int BfsFour(string[] grid, (int x, int y)[] starts, int allMask)
    {
        int h = grid.Length, w = grid[0].Length;
        var init = new State4(starts[0], starts[1], starts[2], starts[3], 0);
        var q = new Queue<(State4 st, int steps)>();
        var seen = new HashSet<State4> { init };
        q.Enqueue((init, 0));

        (int dx, int dy)[] dirs = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        while (q.Count > 0)
        {
            var (st, steps) = q.Dequeue();
            if (st.mask == allMask) return steps;

            (int x, int y)[] curRobots = { st.r0, st.r1, st.r2, st.r3 };
            for (int r = 0; r < 4; r++)
            {
                foreach (var (dx, dy) in dirs)
                {
                    int nx = curRobots[r].x + dx, ny = curRobots[r].y + dy;
                    if (ny < 0 || ny >= h || nx < 0 || nx >= w) continue;
                    char ch = grid[ny][nx];
                    if (ch == '#') continue;

                    if (ch is >= 'A' and <= 'Z')
                    {
                        int doorBit = 1 << (ch - 'A');
                        if ((st.mask & doorBit) == 0) continue;
                    }

                    int newMask = st.mask;
                    if (ch is >= 'a' and <= 'z') newMask |= 1 << (ch - 'a');

                    var nextRobots = ((int x, int y)[])curRobots.Clone();
                    nextRobots[r] = (nx, ny);
                    var nextState = new State4(nextRobots[0], nextRobots[1], nextRobots[2], nextRobots[3], newMask);
                    if (seen.Add(nextState))
                    {
                        q.Enqueue((nextState, steps + 1));
                    }
                }
            }
        }

        return -1;
    }

    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var list = new List<string>();
        foreach (var raw in lines)
        {
            var s = raw.TrimEnd('\r', '\n');
            if (!string.IsNullOrEmpty(s)) list.Add(s);
        }
        string[] grid = list.ToArray();
        int h = grid.Length, w = grid[0].Length;

        int allMask = 0;
        (int x, int y) center = (0, 0);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                char ch = grid[y][x];
                if (ch == '@') center = (x, y);
                else if (ch is >= 'a' and <= 'z') allMask |= 1 << (ch - 'a');
            }
        }

        int p1 = BfsSingle(grid, center, allMask);

        var mut = new char[h][];
        for (int y = 0; y < h; y++) mut[y] = grid[y].ToCharArray();

        string[] patch = { "@#@", "###", "@#@" };
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                mut[center.y + dy][center.x + dx] = patch[dy + 1][dx + 1];
            }
        }

        string[] grid2 = new string[h];
        for (int y = 0; y < h; y++) grid2[y] = new string(mut[y]);

        var robots = new[]
        {
            (center.x - 1, center.y - 1),
            (center.x + 1, center.y - 1),
            (center.x - 1, center.y + 1),
            (center.x + 1, center.y + 1),
        };
        int p2 = BfsFour(grid2, robots, allMask);

        return (p1, p2);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day18_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 18 - Part 1: {p1}, Part 2: {p2}");
    }
}