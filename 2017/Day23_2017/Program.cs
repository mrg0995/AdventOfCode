using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var lines = File.Exists(inputPath)
    ? File.ReadAllLines(inputPath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray()
    : Array.Empty<string>();

var instructions = ParseInstructions(lines);

var mulCount = RunPart1(instructions);
var hValuePart2 = SolvePart2(instructions);

Console.WriteLine($"Parte 1 - Veces que se ejecutó 'mul': {mulCount}");
Console.WriteLine($"Parte 2 - Valor final del registro 'h': {hValuePart2}");

static Instruction[] ParseInstructions(string[] lines)
{
    var list = new List<Instruction>(lines.Length);

    foreach (var raw in lines)
    {
        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var op = parts[0];
        var arg1 = ParseOperand(parts[1]);
        var arg2 = ParseOperand(parts[2]);

        list.Add(new Instruction(op, arg1, arg2));
    }

    return list.ToArray();
}

static Operand ParseOperand(string token)
{
    if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var literal))
        return new Operand(IsRegister: false, Register: '\0', Literal: literal);

    return new Operand(IsRegister: true, Register: token[0], Literal: 0);
}

static long GetValue(Operand operand, long[] registers)
    => operand.IsRegister ? registers[operand.Register - 'a'] : operand.Literal;

static void SetRegister(Operand target, long value, long[] registers)
    => registers[target.Register - 'a'] = value;

static long RunPart1(Instruction[] instructions)
{
    var registers = new long[8];
    long ip = 0;
    long mulExecutions = 0;

    while (ip >= 0 && ip < instructions.Length)
    {
        var inst = instructions[ip];

        switch (inst.Op)
        {
            case "set":
                SetRegister(inst.Arg1, GetValue(inst.Arg2, registers), registers);
                ip++;
                break;

            case "sub":
                SetRegister(inst.Arg1, GetValue(inst.Arg1, registers) - GetValue(inst.Arg2, registers), registers);
                ip++;
                break;

            case "mul":
                SetRegister(inst.Arg1, GetValue(inst.Arg1, registers) * GetValue(inst.Arg2, registers), registers);
                mulExecutions++;
                ip++;
                break;

            case "jnz":
                if (GetValue(inst.Arg1, registers) != 0)
                    ip += GetValue(inst.Arg2, registers);
                else
                    ip++;
                break;

            default:
                throw new InvalidOperationException($"Instrucción desconocida: '{inst.Op}'");
        }
    }

    return mulExecutions;
}

static int SolvePart2(Instruction[] instructions)
{
    if (instructions.Length == 0)
        return 0;

    var firstInst = instructions[0];
    if (firstInst.Op != "set" || !firstInst.Arg1.IsRegister || firstInst.Arg1.Register != 'b' || firstInst.Arg2.IsRegister)
        throw new FormatException("No se pudo extraer el valor inicial de 'b' desde la primera instrucción.");

    var rawB = firstInst.Arg2.Literal;
    var startB = rawB * 100 + 100_000;
    var endB = startB + 17_000;

    var nonPrimes = 0;
    for (var b = startB; b <= endB; b += 17)
    {
        if (!IsPrime(b))
            nonPrimes++;
    }

    return nonPrimes;
}

static bool IsPrime(long n)
{
    if (n < 2)
        return false;
    if (n % 2 == 0)
        return n == 2;

    for (long d = 3; d * d <= n; d += 2)
    {
        if (n % d == 0)
            return false;
    }

    return true;
}

readonly record struct Operand(bool IsRegister, char Register, long Literal);
readonly record struct Instruction(string Op, Operand Arg1, Operand Arg2);