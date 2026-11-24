using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.ReadAllLines(inputPath)
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .ToArray();

var nodes = ParseTower(lines);

var allChildren = new HashSet<string>(
    nodes.Values.SelectMany(n => n.ChildNames)
);
var root = nodes.Values.First(n => !allChildren.Contains(n.Name));

Console.WriteLine($"Parte 1 - Programa base: {root.Name}");

var targetWeight = BalanceTower(root, nodes, delta: 0);
Console.WriteLine($"Parte 2 - Peso corregido: {targetWeight}");

static Dictionary<string, ProgramNode> ParseTower(string[] lines)
{
    var regex = new Regex(@"^(?<name>\w+)\s+\((?<weight>\d+)\)(?:\s+->\s+(?<children>.+))?$", RegexOptions.Compiled);
    var nodes = new Dictionary<string, ProgramNode>();

    foreach (var line in lines)
    {
        var match = regex.Match(line.Trim());
        if (!match.Success)
            throw new FormatException($"Línea inválida: {line}");

        var name = match.Groups["name"].Value;
        var weight = int.Parse(match.Groups["weight"].Value);
        var children = match.Groups["children"].Success
            ? match.Groups["children"].Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();

        nodes[name] = new ProgramNode(name, weight, children);
    }

    return nodes;
}

static int GetTotalWeight(string nodeName, Dictionary<string, ProgramNode> nodes, Dictionary<string, int> cache)
{
    if (cache.TryGetValue(nodeName, out var cached))
        return cached;

    var node = nodes[nodeName];
    var total = node.Weight + node.ChildNames.Sum(child => GetTotalWeight(child, nodes, cache));
    cache[nodeName] = total;
    return total;
}

static int BalanceTower(ProgramNode current, Dictionary<string, ProgramNode> nodes, int delta)
{
    var cache = new Dictionary<string, int>();
    var childWeights = current.ChildNames
        .Select(child => (Name: child, Total: GetTotalWeight(child, nodes, cache)))
        .ToList();

    var groups = childWeights
        .GroupBy(x => x.Total)
        .OrderByDescending(g => g.Count())
        .ToList();

    if (groups.Count <= 1)
    {
        return current.Weight + delta;
    }

    var majorityWeight = groups[0].Key;
    var oddGroup = groups[1];
    var oddChildName = oddGroup.First().Name;
    var oddTotalWeight = oddGroup.Key;

    var nextDelta = majorityWeight - oddTotalWeight;

    return BalanceTower(nodes[oddChildName], nodes, nextDelta);
}

record ProgramNode(string Name, int Weight, string[] ChildNames);