using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day24
{
    static long Biodiversity(bool[,] grid)
    {
        long score = 0;
        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                if (grid[r, c]) score += 1L << (r * 5 + c);
            }
        }
        return score;
    }

    static bool[,] StepP1(bool[,] grid)
    {
        var nxt = new bool[5, 5];
        (int dr, int dc)[] dirs = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                int adj = 0;
                foreach (var (dr, dc) in dirs)
                {
                    int nr = r + dr, nc = c + dc;
                    if (nr is >= 0 and < 5 && nc is >= 0 and < 5 && grid[nr, nc]) adj++;
                }
                nxt[r, c] = grid[r, c] ? (adj == 1) : (adj is 1 or 2);
            }
        }
        return nxt;
    }

    static int AdjacentBugsP2(Dictionary<int, bool[,]> levels, int lvl, int r, int c)
    {
        int count = 0;
        (int dr, int dc)[] dirs = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        foreach (var (dr, dc) in dirs)
        {
            int nr = r + dr, nc = c + dc;
            if (nr == 2 && nc == 2)
            {
                if (levels.TryGetValue(lvl + 1, out var inner))
                {
                    if (r == 1 && c == 2)
                        for (int x = 0; x < 5; x++) if (inner[0, x]) count++;
                        else if (r == 3 && c == 2)
                            for (int x = 0; x < 5; x++) if (inner[4, x]) count++;
                            else if (r == 2 && c == 1)
                                for (int y = 0; y < 5; y++) if (inner[y, 0]) count++;
                                else if (r == 2 && c == 3)
                                    for (int y = 0; y < 5; y++) if (inner[y, 4]) count++;
                }
            }
            else if (nr < 0)
            {
                if (levels.TryGetValue(lvl - 1, out var outer) && outer[1, 2]) count++;
            }
            else if (nr >= 5)
            {
                if (levels.TryGetValue(lvl - 1, out var outer) && outer[3, 2]) count++;
            }
            else if (nc < 0)
            {
                if (levels.TryGetValue(lvl - 1, out var outer) && outer[2, 1]) count++;
            }
            else if (nc >= 5)
            {
                if (levels.TryGetValue(lvl - 1, out var outer) && outer[2, 3]) count++;
            }
            else if (levels.TryGetValue(lvl, out var cur) && cur[nr, nc])
            {
                count++;
            }
        }

        return count;
    }

    public static (long Part1, int Part2) Solve(string[] lines)
    {
        var rows = lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToArray();
        var init = new bool[5, 5];
        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                init[r, c] = rows[r][c] == '#';
            }
        }

        var cur = (bool[,])init.Clone();
        var seen = new HashSet<long> { Biodiversity(cur) };
        long p1 = 0;

        while (true)
        {
            cur = StepP1(cur);
            long b = Biodiversity(cur);
            if (!seen.Add(b))
            {
                p1 = b;
                break;
            }
        }

        var levels = new Dictionary<int, bool[,]> { [0] = (bool[,])init.Clone() };
        for (int minute = 0; minute < 200; minute++)
        {
            int minL = levels.Keys.Min() - 1;
            int maxL = levels.Keys.Max() + 1;
            var nextLevels = new Dictionary<int, bool[,]>();

            for (int lvl = minL; lvl <= maxL; lvl++)
            {
                var ng = new bool[5, 5];
                levels.TryGetValue(lvl, out var curLvl);

                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        if (r == 2 && c == 2) continue;
                        int adj = AdjacentBugsP2(levels, lvl, r, c);
                        bool hasBug = curLvl != null && curLvl[r, c];
                        ng[r, c] = hasBug ? (adj == 1) : (adj is 1 or 2);
                    }
                }

                nextLevels[lvl] = ng;
            }

            levels = nextLevels;
        }

        int p2 = 0;
        foreach (var g in levels.Values)
        {
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (!(r == 2 && c == 2) && g[r, c]) p2++;
                }
            }
        }

        return (p1, p2);
    }

    static void Main()
    {
        string[] lines = File.ReadAllLines("Day24_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 24 - Part 1: {p1}, Part 2: {p2}");
    }
}
