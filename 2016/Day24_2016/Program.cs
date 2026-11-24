using System;
using System.Collections.Generic;
using System.IO;

class AdventOfCode2016Day24
{
    static void Main()
    {
        string[] grid = File.ReadAllLines("Your_Route");

        var (part1, part2) = Solve(grid);

        Console.WriteLine($"Parte 1: {part1}");
        Console.WriteLine($"Parte 2: {part2}");
    }

    static (int part1, int part2) Solve(string[] grid)
    {
        int height = grid.Length;
        int width = grid[0].Length;

        var points = new SortedDictionary<int, (int r, int c)>();
        for (int r = 0; r < height; r++)
        {
            string row = grid[r];
            for (int c = 0; c < width; c++)
            {
                char ch = row[c];
                if (ch is >= '0' and <= '9')
                {
                    points[ch - '0'] = (r, c);
                }
            }
        }

        int n = points.Count;
        var coords = new (int r, int c)[n];
        for (int i = 0; i < n; i++)
            coords[i] = points[i];

        int[,] dist = new int[n, n];
        int totalCells = height * width;
        int[] cellDist = new int[totalCells];
        int[] queue = new int[totalCells];

        ReadOnlySpan<int> dr = stackalloc int[] { -1, 1, 0, 0 };
        ReadOnlySpan<int> dc = stackalloc int[] { 0, 0, -1, 1 };

        for (int i = 0; i < n; i++)
        {
            Array.Fill(cellDist, -1);

            int startIdx = coords[i].r * width + coords[i].c;
            cellDist[startIdx] = 0;

            int head = 0, tail = 0;
            queue[tail++] = startIdx;

            while (head < tail)
            {
                int curr = queue[head++];
                int r = curr / width;
                int c = curr % width;
                int nextD = cellDist[curr] + 1;

                for (int dir = 0; dir < 4; dir++)
                {
                    int nr = r + dr[dir];
                    int nc = c + dc[dir];

                    if ((uint)nr >= (uint)height || (uint)nc >= (uint)width)
                        continue;

                    if (grid[nr][nc] == '#')
                        continue;

                    int nextIdx = nr * width + nc;
                    if (cellDist[nextIdx] == -1)
                    {
                        cellDist[nextIdx] = nextD;
                        queue[tail++] = nextIdx;
                    }
                }
            }

            for (int j = 0; j < n; j++)
            {
                int targetIdx = coords[j].r * width + coords[j].c;
                dist[i, j] = cellDist[targetIdx];
            }
        }

        int fullMask = (1 << n) - 1;
        const int INF = 1_000_000_000;
        int[,] dp = new int[1 << n, n];

        for (int m = 0; m <= fullMask; m++)
            for (int i = 0; i < n; i++)
                dp[m, i] = INF;

        dp[1, 0] = 0;

        for (int mask = 1; mask <= fullMask; mask += 2)
        {
            for (int u = 0; u < n; u++)
            {
                if ((mask & (1 << u)) == 0) continue;
                int currentCost = dp[mask, u];
                if (currentCost >= INF) continue;

                for (int v = 1; v < n; v++)
                {
                    if ((mask & (1 << v)) != 0) continue;
                    int nextMask = mask | (1 << v);
                    int nextCost = currentCost + dist[u, v];
                    if (nextCost < dp[nextMask, v])
                        dp[nextMask, v] = nextCost;
                }
            }
        }

        int part1 = INF;
        int part2 = INF;

        for (int last = 1; last < n; last++)
        {
            int cost = dp[fullMask, last];
            if (cost < part1)
                part1 = cost;

            int returnCost = cost + dist[last, 0];
            if (returnCost < part2)
                part2 = returnCost;
        }

        return (part1, part2);
    }
}
