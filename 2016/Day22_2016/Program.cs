using System;
using System.Collections.Generic;
using System.IO;

class AdventOfCode2016Day22
{
    readonly record struct Node(int X, int Y, int Size, int Used, int Avail);

    static void Main()
    {
        string[] lines = File.ReadAllLines("Your_Route");
        var nodes = ParseNodes(lines);

        int part1 = SolvePart1(nodes);
        int part2 = SolvePart2(nodes);

        Console.WriteLine($"Parte 1: {part1}");
        Console.WriteLine($"Parte 2: {part2}");
    }

    static List<Node> ParseNodes(string[] lines)
    {
        var nodes = new List<Node>(lines.Length);

        foreach (string rawLine in lines)
        {
            ReadOnlySpan<char> line = rawLine.AsSpan().Trim();
            if (!line.StartsWith("/dev/grid/node-x")) continue;

            int xStart = "/dev/grid/node-x".Length;
            int yMarker = line.IndexOf("-y");
            int x = int.Parse(line.Slice(xStart, yMarker - xStart));

            int yStart = yMarker + 2;
            int spaceAfterY = line.Slice(yStart).IndexOf(' ') + yStart;
            int y = int.Parse(line.Slice(yStart, spaceAfterY - yStart));

            ReadOnlySpan<char> rest = line.Slice(spaceAfterY);
            int size = ReadNextNumberWithUnit(ref rest);
            int used = ReadNextNumberWithUnit(ref rest);
            int avail = ReadNextNumberWithUnit(ref rest);

            nodes.Add(new Node(x, y, size, used, avail));
        }

        return nodes;
    }

    static int ReadNextNumberWithUnit(ref ReadOnlySpan<char> span)
    {
        span = span.TrimStart();
        int tIndex = span.IndexOf('T');
        int value = int.Parse(span.Slice(0, tIndex));
        span = span.Slice(tIndex + 1);
        return value;
    }

    static int SolvePart1(List<Node> nodes)
    {
        int n = nodes.Count;
        int[] avails = new int[n];
        for (int i = 0; i < n; i++)
            avails[i] = nodes[i].Avail;

        Array.Sort(avails);

        int viablePairs = 0;
        foreach (var a in nodes)
        {
            if (a.Used == 0) continue;

            int left = 0, right = n;
            while (left < right)
            {
                int mid = left + ((right - left) >> 1);
                if (avails[mid] < a.Used) left = mid + 1;
                else right = mid;
            }

            int count = n - left;
            if (a.Avail >= a.Used) count--;

            viablePairs += count;
        }

        return viablePairs;
    }

    static int SolvePart2(List<Node> nodes)
    {
        int maxX = 0, maxY = 0;
        foreach (var n in nodes)
        {
            if (n.X > maxX) maxX = n.X;
            if (n.Y > maxY) maxY = n.Y;
        }

        int width = maxX + 1;
        int height = maxY + 1;

        int emptyX = -1, emptyY = -1;
        int emptyCapacity = 0;

        foreach (var n in nodes)
        {
            if (n.Used == 0)
            {
                emptyX = n.X;
                emptyY = n.Y;
                emptyCapacity = n.Size;
                break;
            }
        }

        bool[] isWall = new bool[width * height];
        foreach (var n in nodes)
        {
            if (n.Used > emptyCapacity)
                isWall[n.Y * width + n.X] = true;
        }

        int goalX = maxX;
        int goalY = 0;

        int totalStates = width * height * width * height;
        var visited = new bool[totalStates];

        int Encode(int ex, int ey, int gx, int gy) =>
            (((ey * width + ex) * height + gy) * width + gx);

        var queue = new Queue<(int ex, int ey, int gx, int gy, int steps)>(4096);
        int startState = Encode(emptyX, emptyY, goalX, goalY);
        visited[startState] = true;
        queue.Enqueue((emptyX, emptyY, goalX, goalY, 0));

        ReadOnlySpan<int> dx = stackalloc int[] { 0, 0, -1, 1 };
        ReadOnlySpan<int> dy = stackalloc int[] { -1, 1, 0, 0 };

        while (queue.Count > 0)
        {
            var (ex, ey, gx, gy, steps) = queue.Dequeue();

            if (gx == 0 && gy == 0)
                return steps;

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = ex + dx[dir];
                int ny = ey + dy[dir];

                if ((uint)nx >= (uint)width || (uint)ny >= (uint)height)
                    continue;

                if (isWall[ny * width + nx])
                    continue;

                int ngx = (nx == gx && ny == gy) ? ex : gx;
                int ngy = (nx == gx && ny == gy) ? ey : gy;

                int state = Encode(nx, ny, ngx, ngy);
                if (!visited[state])
                {
                    visited[state] = true;
                    queue.Enqueue((nx, ny, ngx, ngy, steps + 1));
                }
            }
        }

        throw new InvalidOperationException("No se encontró ruta para mover los datos a (0,0).");
    }
}