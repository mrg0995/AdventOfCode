using System;
using System.IO;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var stream = File.ReadAllText(inputPath).Trim();

var (totalScore, garbageCount) = ProcessStream(stream);

Console.WriteLine($"Parte 1 - Puntuación total de grupos: {totalScore}");
Console.WriteLine($"Parte 2 - Caracteres de basura no cancelados: {garbageCount}");

static (int TotalScore, int GarbageCount) ProcessStream(string stream)
{
    var depth = 0;
    var totalScore = 0;
    var garbageCount = 0;
    var inGarbage = false;
    var ignoreNext = false;

    foreach (var ch in stream)
    {
        if (ignoreNext)
        {
            ignoreNext = false;
            continue;
        }

        if (ch == '!')
        {
            ignoreNext = true;
            continue;
        }

        if (inGarbage)
        {
            if (ch == '>')
            {
                inGarbage = false;
            }
            else
            {
                garbageCount++;
            }
            continue;
        }

        switch (ch)
        {
            case '<':
                inGarbage = true;
                break;
            case '{':
                depth++;
                totalScore += depth;
                break;
            case '}':
                if (depth > 0)
                    depth--;
                break;
        }
    }

    return (totalScore, garbageCount);
}
