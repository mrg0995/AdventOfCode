using System;
using System.Collections.Generic;
using System.IO;

string[] lines = File.ReadAllLines("Your_Route");
Console.WriteLine($"Parte 1: {RunProgram(lines, initialC: 0)}");
Console.WriteLine($"Parte 2: {RunProgram(lines, initialC: 1)}");

static int GetValue(string operand, Dictionary<string, int> regs) =>
        regs.TryGetValue(operand, out int val) ? val : int.Parse(operand);

static int RunProgram(string[] lines, int initialC = 0)
{
    var instructions = new List<string[]>();
    foreach (var raw in lines)
    {
        var trimmed = raw.Trim();
        if (!string.IsNullOrEmpty(trimmed))
            instructions.Add(trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    var regs = new Dictionary<string, int>
    {
        ["a"] = 0,
        ["b"] = 0,
        ["c"] = initialC,
        ["d"] = 0
    };

    int ip = 0;
    while (ip >= 0 && ip < instructions.Count)
    {
        var parts = instructions[ip];
        switch (parts[0])
        {
            case "cpy":
                regs[parts[2]] = GetValue(parts[1], regs);
                ip++;
                break;
            case "inc":
                regs[parts[1]]++;
                ip++;
                break;
            case "dec":
                regs[parts[1]]--;
                ip++;
                break;
            case "jnz":
                int cond = GetValue(parts[1], regs);
                int offset = GetValue(parts[2], regs);
                ip += cond != 0 ? offset : 1;
                break;
            default:
                throw new InvalidOperationException($"Instrucción desconocida: {parts[0]}");
        }
    }

    return regs["a"];
}