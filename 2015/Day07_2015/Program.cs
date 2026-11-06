using System;
using System.Collections.Generic;
using System.IO;

Dictionary<string, string> instructions = new Dictionary<string, string>();
Dictionary<string, ushort> memo = new Dictionary<string, ushort>();

string filePath = "C:\\Users\\ProtoMkUser\\Desktop\\Day7_2015.txt";

if (!File.Exists(filePath))
{
    Console.WriteLine($"No se encontró el archivo '{filePath}'. Asegúrate de crearlo en la misma carpeta con tu puzzle input.");
    return;
}

using (StreamReader reader = new StreamReader(filePath))
{
    string line;
    while ((line = reader.ReadLine()) != null)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        var parts = line.Split(new[] { " -> " }, StringSplitOptions.None);
        if (parts.Length == 2)
        {
            string formula = parts[0].Trim();
            string targetWire = parts[1].Trim();
            instructions[targetWire] = formula;
        }
    }
}

try
{
    ushort signalA = GetSignal("a");
    Console.WriteLine($"¡La señal enviada al cable 'a' es: {signalA}");

    memo.Clear();
    instructions["b"] = signalA.ToString();
    ushort newSignalA = GetSignal("a");
    Console.WriteLine($"Resultado Parte 2 (Cable a): {newSignalA}");
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al procesar el circuito: {ex.Message}");
}

ushort GetSignal(string wire)
{
    if (ushort.TryParse(wire, out ushort directValue))
    {
        return directValue;
    }

    if (memo.ContainsKey(wire))
    {
        return memo[wire];
    }

    if (!instructions.ContainsKey(wire))
    {
        throw new Exception($"El cable '{wire}' no está definido en las instrucciones.");
    }

    string expr = instructions[wire];
    ushort result = 0;
    var tokens = expr.Split(' ');

    if (tokens.Length == 1)
    {
        result = GetSignal(tokens[0]);
    }
    else if (tokens.Length == 2 && tokens[0] == "NOT")
    {

        result = (ushort)~GetSignal(tokens[1]);
    }
    else if (tokens.Length == 3)
    {
        string op = tokens[1];
        ushort left = GetSignal(tokens[0]);

        if (!ushort.TryParse(tokens[2], out ushort right))
        {
            right = GetSignal(tokens[2]);
        }

        switch (op)
        {
            case "AND":
                result = (ushort)(left & right);
                break;
            case "OR":
                result = (ushort)(left | right);
                break;
            case "LSHIFT":
                result = (ushort)(left << right);
                break;
            case "RSHIFT":
                result = (ushort)(left >> right);
                break;
            default:
                throw new ArgumentException($"Operador desconocido: {op}");
        }
    }

    memo[wire] = result;
    return result;
}

