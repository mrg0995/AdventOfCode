using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : Array.Empty<string>();

var layers = ParseLayers(lines);

var severity = CalculateSeverity(layers);
var safeDelay = FindSafeDelay(layers);

Console.WriteLine($"Parte 1 - Severidad del recorrido (delay 0): {severity}");
Console.WriteLine($"Parte 2 - Menor retraso seguro: {safeDelay}");

static List<FirewallLayer> ParseLayers(string[] lines)
{
    var layers = new List<FirewallLayer>(lines.Length);

    foreach (var line in lines)
    {
        var parts = line.Split(':', StringSplitOptions.TrimEntries);
        var depth = int.Parse(parts[0]);
        var range = int.Parse(parts[1]);
        var period = range > 1 ? 2 * (range - 1) : 1;

        layers.Add(new FirewallLayer(depth, range, period));
    }

    return layers;
}

static int CalculateSeverity(List<FirewallLayer> layers)
{
    var severity = 0;

    foreach (var layer in layers)
    {
        if (layer.Depth % layer.Period == 0)
        {
            severity += layer.Depth * layer.Range;
        }
    }

    return severity;
}

static int FindSafeDelay(List<FirewallLayer> layers)
{
    var orderedLayers = layers.OrderByDescending(l => l.Period).ToArray();

    for (var delay = 0; ; delay++)
    {
        var caught = false;

        foreach (var layer in orderedLayers)
        {
            if ((delay + layer.Depth) % layer.Period == 0)
            {
                caught = true;
                break;
            }
        }

        if (!caught)
            return delay;
    }
}

readonly record struct FirewallLayer(int Depth, int Range, int Period);