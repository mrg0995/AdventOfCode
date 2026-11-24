using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.ReadAllLines(inputPath)
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .ToArray();

var (maxAtEnd, maxEver) = RunInstructions(lines);

Console.WriteLine($"Parte 1 - Mayor valor al final: {maxAtEnd}");
Console.WriteLine($"Parte 2 - Mayor valor histórico: {maxEver}");

static (int MaxAtEnd, int MaxEver) RunInstructions(string[] lines)
{
    var registers = new Dictionary<string, int>();
    var maxEver = int.MinValue;

    foreach (var line in lines)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var targetReg = parts[0];
        var operation = parts[1];
        var amount = int.Parse(parts[2]);
        var condReg = parts[4];
        var condOp = parts[5];
        var condVal = int.Parse(parts[6]);

        var currentCondValue = registers.GetValueOrDefault(condReg, 0);

        if (EvaluateCondition(currentCondValue, condOp, condVal))
        {
            var currentTargetValue = registers.GetValueOrDefault(targetReg, 0);
            var newValue = operation switch
            {
                "inc" => currentTargetValue + amount,
                "dec" => currentTargetValue - amount,
                _ => throw new InvalidOperationException($"Operación desconocida: {operation}")
            };

            registers[targetReg] = newValue;

            if (newValue > maxEver)
                maxEver = newValue;
        }
    }

    var maxAtEnd = registers.Count > 0 ? registers.Values.Max() : 0;
    if (maxEver == int.MinValue) maxEver = 0;

    return (maxAtEnd, maxEver);
}

static bool EvaluateCondition(int left, string op, int right) => op switch
{
    ">" => left > right,
    "<" => left < right,
    ">=" => left >= right,
    "<=" => left <= right,
    "==" => left == right,
    "!=" => left != right,
    _ => throw new InvalidOperationException($"Operador de condición desconocido: {op}")
};