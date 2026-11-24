using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : Array.Empty<string>();

var graph = ParseGraph(lines);

var groupContainingZero = GetConnectedComponent(0, graph);
var totalGroups = CountConnectedComponents(graph);

Console.WriteLine($"Parte 1 - Programas en el grupo del 0: {groupContainingZero.Count}");
Console.WriteLine($"Parte 2 - Número total de grupos: {totalGroups}");

static Dictionary<int, List<int>> ParseGraph(string[] lines)
{
    var graph = new Dictionary<int, List<int>>();

    foreach (var line in lines)
    {
        var parts = line.Split("<->", StringSplitOptions.TrimEntries);
        var node = int.Parse(parts[0]);

        if (!graph.ContainsKey(node))
            graph[node] = new List<int>();

        var neighbors = parts[1]
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse);

        foreach (var neighbor in neighbors)
        {
            if (!graph.ContainsKey(neighbor))
                graph[neighbor] = new List<int>();

            if (!graph[node].Contains(neighbor))
                graph[node].Add(neighbor);

            if (!graph[neighbor].Contains(node))
                graph[neighbor].Add(node);
        }
    }

    return graph;
}

static HashSet<int> GetConnectedComponent(int startNode, Dictionary<int, List<int>> graph)
{
    var visited = new HashSet<int>();
    if (!graph.ContainsKey(startNode))
        return visited;

    var queue = new Queue<int>();
    visited.Add(startNode);
    queue.Enqueue(startNode);

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        foreach (var neighbor in graph[current])
        {
            if (visited.Add(neighbor))
            {
                queue.Enqueue(neighbor);
            }
        }
    }

    return visited;
}

static int CountConnectedComponents(Dictionary<int, List<int>> graph)
{
    var globalVisited = new HashSet<int>();
    var groupCount = 0;

    foreach (var node in graph.Keys)
    {
        if (globalVisited.Contains(node))
            continue;

        groupCount++;
        var component = GetConnectedComponent(node, graph);
        foreach (var member in component)
        {
            globalVisited.Add(member);
        }
    }

    return groupCount;
}