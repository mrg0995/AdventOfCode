using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Program
{
    static readonly (int dx, int dy)[] Directions =
    {
        (1, 0), (-1, 0), (0, 1), (0, -1)
    };

    static bool IsOpen(int x, int y, int favoriteNumber)
    {
        if (x < 0 || y < 0)
            return false;

        long value = (long)x * x + 3L * x + 2L * x * y + y + (long)y * y + favoriteNumber;
        int ones = BitOperations.PopCount((ulong)value);
        return ones % 2 == 0;
    }

    static int SolvePart1(int favoriteNumber, (int x, int y) target)
    {
        var start = (x: 1, y: 1);
        var queue = new Queue<(int x, int y, int steps)>();
        var visited = new HashSet<(int, int)> { start };

        queue.Enqueue((start.x, start.y, 0));

        while (queue.Count > 0)
        {
            var (x, y, steps) = queue.Dequeue();
            if ((x, y) == target)
                return steps;

            foreach (var (dx, dy) in Directions)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (!visited.Contains((nx, ny)) && IsOpen(nx, ny, favoriteNumber))
                {
                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny, steps + 1));
                }
            }
        }

        throw new InvalidOperationException("No se encontró camino hasta el objetivo.");
    }

    static int SolvePart2(int favoriteNumber, int maxSteps = 50)
    {
        var start = (x: 1, y: 1);
        var queue = new Queue<(int x, int y, int steps)>();
        var visited = new HashSet<(int, int)> { start };

        queue.Enqueue((start.x, start.y, 0));

        while (queue.Count > 0)
        {
            var (x, y, steps) = queue.Dequeue();
            if (steps == maxSteps)
                continue;

            foreach (var (dx, dy) in Directions)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (!visited.Contains((nx, ny)) && IsOpen(nx, ny, favoriteNumber))
                {
                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny, steps + 1));
                }
            }
        }

        return visited.Count;
    }

    static void Main()
    {
        int favoriteNumber = 1362;
        Console.WriteLine($"Parte 1: {SolvePart1(favoriteNumber, (31, 39))}");
        Console.WriteLine($"Parte 2: {SolvePart2(favoriteNumber)}");
    }
}