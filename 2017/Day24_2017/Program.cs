using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : GetSampleComponents();

var components = ParseComponents(lines);
var used = new bool[components.Length];

var bestStrength = 0;
var bestLength = 0;
var bestStrengthForLongest = 0;

SearchBridges(
    currentPort: 0,
    currentLength: 0,
    currentStrength: 0,
    components,
    used,
    ref bestStrength,
    ref bestLength,
    ref bestStrengthForLongest);

Console.WriteLine($"Parte 1 - Fuerza máxima del puente: {bestStrength}");
Console.WriteLine($"Parte 2 - Fuerza del puente más largo (longitud {bestLength}): {bestStrengthForLongest}");

static Component[] ParseComponents(string[] lines)
{
    var result = new Component[lines.Length];

    for (var i = 0; i < lines.Length; i++)
    {
        var parts = lines[i].Split('/', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            throw new FormatException($"Componente inválido en línea {i + 1}: '{lines[i]}'");

        var portA = int.Parse(parts[0], CultureInfo.InvariantCulture);
        var portB = int.Parse(parts[1], CultureInfo.InvariantCulture);
        result[i] = new Component(portA, portB, portA + portB);
    }

    return result;
}

static void SearchBridges(
    int currentPort,
    int currentLength,
    int currentStrength,
    Component[] components,
    bool[] used,
    ref int bestStrength,
    ref int bestLength,
    ref int bestStrengthForLongest)
{
    if (currentStrength > bestStrength)
    {
        bestStrength = currentStrength;
    }

    if (currentLength > bestLength ||
        (currentLength == bestLength && currentStrength > bestStrengthForLongest))
    {
        bestLength = currentLength;
        bestStrengthForLongest = currentStrength;
    }

    for (var i = 0; i < components.Length; i++)
    {
        if (used[i])
            continue;

        var comp = components[i];
        int? nextPort = null;

        if (comp.PortA == currentPort)
            nextPort = comp.PortB;
        else if (comp.PortB == currentPort)
            nextPort = comp.PortA;

        if (nextPort is null)
            continue;

        used[i] = true;

        SearchBridges(
            currentPort: nextPort.Value,
            currentLength: currentLength + 1,
            currentStrength: currentStrength + comp.Strength,
            components,
            used,
            ref bestStrength,
            ref bestLength,
            ref bestStrengthForLongest);

        used[i] = false;
    }
}

static string[] GetSampleComponents() =>
[
    "0/2",
    "2/2",
    "2/3",
    "3/4",
    "3/5",
    "0/1",
    "10/1",
    "9/10"
];

readonly record struct Component(int PortA, int PortB, int Strength);