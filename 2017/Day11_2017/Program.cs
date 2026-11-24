using System;
using System.IO;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var rawInput = File.Exists(inputPath)
    ? File.ReadAllText(inputPath).Trim()
    : string.Empty;

var steps = rawInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var (finalDistance, maxDistance) = TraceHexPath(steps);

Console.WriteLine($"Parte 1 - Distancia final: {finalDistance}");
Console.WriteLine($"Parte 2 - Distancia máxima alcanzada: {maxDistance}");

static (int FinalDistance, int MaxDistance) TraceHexPath(string[] steps)
{
    var x = 0;
    var y = 0;
    var z = 0;
    var maxDistance = 0;

    foreach (var step in steps)
    {
        switch (step)
        {
            case "n":
                y += 1;
                z -= 1;
                break;
            case "ne":
                x += 1;
                z -= 1;
                break;
            case "se":
                x += 1;
                y -= 1;
                break;
            case "s":
                y -= 1;
                z += 1;
                break;
            case "sw":
                x -= 1;
                z += 1;
                break;
            case "nw":
                x -= 1;
                y += 1;
                break;
            default:
                throw new InvalidOperationException($"Dirección desconocida: '{step}'");
        }

        var currentDistance = HexDistance(x, y, z);
        if (currentDistance > maxDistance)
        {
            maxDistance = currentDistance;
        }
    }

    var finalDistance = HexDistance(x, y, z);
    return (finalDistance, maxDistance);
}

static int HexDistance(int x, int y, int z)
    => (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)) / 2;