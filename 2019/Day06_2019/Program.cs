using System;
using System.Collections.Generic;
using System.IO;

static class Day06
{
    public static (int Part1, int Part2) Solve(string[] lines)
    {
        var parent = new Dictionary<string, string>();
        var adj = new Dictionary<string, List<string>>();

        void AddEdge(string u, string v)
        {
            if (!adj.TryGetValue(u, out var list))
            {
                list = new List<string>();
                adj[u] = list;
            }
            list.Add(v);
        }

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split(')');
            string center = parts[0], orbiter = parts[1];
            parent[orbiter] = center;
            AddEdge(center, orbiter);
            AddEdge(orbiter, center);
        }

        int totalOrbits = 0;
        foreach (var node in parent.Keys)
        {
            string curr = node;
            while (parent.TryGetValue(curr, out var p))
            {
                totalOrbits++;
                curr = p;
            }
        }

        string start = parent["YOU"];
        string goal = parent["SAN"];
        var queue = new Queue<(string node, int dist)>();
        var visited = new HashSet<string> { start };
        queue.Enqueue((start, 0));

        int transfers = -1;
        while (queue.Count > 0)
        {
            var (curr, dist) = queue.Dequeue();
            if (curr == goal)
            {
                transfers = dist;
                break;
            }

            if (!adj.TryGetValue(curr, out var neighbors)) continue;
            foreach (var nxt in neighbors)
            {
                if (visited.Add(nxt))
                {
                    queue.Enqueue((nxt, dist + 1));
                }
            }
        }

        return (totalOrbits, transfers);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day06_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 6 - Part 1: {p1}, Part 2: {p2}");
    }
}
