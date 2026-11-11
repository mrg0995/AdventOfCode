using System;
using System.IO;

string[] instructions = File.ReadAllLines(@"Your_Route");

long resultPart1 = RunProgram(instructions, initialA: 0);
Console.WriteLine($"Parte 1 - Valor final del registro b: {resultPart1}");

long resultPart2 = RunProgram(instructions, initialA: 1);
Console.WriteLine($"Parte 2 - Valor final del registro b: {resultPart2}");

static long RunProgram(string[] instructions, long initialA)
{
    long a = initialA;
    long b = 0;
    int pc = 0;

    while (pc >= 0 && pc < instructions.Length)
    {
        string line = instructions[pc];
        string op = line.Substring(0, 3);

        switch (op)
        {
            case "hlf":
                char regHlf = line[4];
                if (regHlf == 'a') a /= 2;
                else b /= 2;
                pc++;
                break;

            case "tpl":
                char regTpl = line[4];
                if (regTpl == 'a') a *= 3;
                else b *= 3;
                pc++;
                break;

            case "inc":
                char regInc = line[4];
                if (regInc == 'a') a++;
                else b++;
                pc++;
                break;

            case "jmp":
                int offsetJmp = int.Parse(line.Substring(4));
                pc += offsetJmp;
                break;

            case "jie":
                char regJie = line[4];
                int offsetJie = int.Parse(line.Substring(7));
                long valJie = (regJie == 'a') ? a : b;
                if (valJie % 2 == 0)
                    pc += offsetJie;
                else
                    pc++;
                break;

            case "jio":
                char regJio = line[4];
                int offsetJio = int.Parse(line.Substring(7));
                long valJio = (regJio == 'a') ? a : b;
                if (valJio == 1)
                    pc += offsetJio;
                else
                    pc++;
                break;
        }
    }

    return b;
}