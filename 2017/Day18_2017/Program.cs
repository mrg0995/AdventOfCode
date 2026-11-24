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

var recoveredSound = RunPart1(instructions);
var program1Sends = RunPart2(instructions);

Console.WriteLine($"Parte 1 - Primer valor recuperado (rcv): {recoveredSound}");
Console.WriteLine($"Parte 2 - Veces que el programa 1 envió un valor: {program1Sends}");

static Instruction[] ParseInstructions(string[] lines)
{
    var list = new List<Instruction>(lines.Length);

    foreach (var raw in lines)
    {
        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var op = parts[0];
        var arg1 = ParseOperand(parts[1]);
        var arg2 = parts.Length > 2 ? ParseOperand(parts[2]) : default;

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

static long RunPart1(Instruction[] instructions)
{
    var registers = new Dictionary<char, long>();
    long ip = 0;
    long lastSound = 0;

    while (ip >= 0 && ip < instructions.Length)
    {
        var inst = instructions[ip];

        switch (inst.Op)
        {
            case "snd":
                lastSound = DuetProgram.GetValue(inst.Arg1, registers);
                ip++;
                break;

            case "set":
                registers[inst.Arg1.Register] = DuetProgram.GetValue(inst.Arg2, registers);
                ip++;
                break;

            case "add":
                registers[inst.Arg1.Register] =
                    registers.GetValueOrDefault(inst.Arg1.Register, 0L) + DuetProgram.GetValue(inst.Arg2, registers);
                ip++;
                break;

            case "mul":
                registers[inst.Arg1.Register] =
                    registers.GetValueOrDefault(inst.Arg1.Register, 0L) * DuetProgram.GetValue(inst.Arg2, registers);
                ip++;
                break;

            case "mod":
                registers[inst.Arg1.Register] =
                    registers.GetValueOrDefault(inst.Arg1.Register, 0L) % DuetProgram.GetValue(inst.Arg2, registers);
                ip++;
                break;

            case "rcv":
                if (DuetProgram.GetValue(inst.Arg1, registers) != 0)
                    return lastSound;
                ip++;
                break;

            case "jgz":
                if (DuetProgram.GetValue(inst.Arg1, registers) > 0)
                    ip += DuetProgram.GetValue(inst.Arg2, registers);
                else
                    ip++;
                break;

            default:
                throw new InvalidOperationException($"Instrucción desconocida: '{inst.Op}'");
        }
    }

    return lastSound;
}

static long RunPart2(Instruction[] instructions)
{
    var prog0 = new DuetProgram(id: 0, instructions);
    var prog1 = new DuetProgram(id: 1, instructions);

    while (true)
    {
        var progressed0 = prog0.Step(incoming: prog0.Queue, outgoing: prog1.Queue);
        var progressed1 = prog1.Step(incoming: prog1.Queue, outgoing: prog0.Queue);

        if (!progressed0 && !progressed1)
            break;
    }

    return prog1.SendCount;
}

sealed class DuetProgram
{
    private readonly Instruction[] _instructions;
    private readonly Dictionary<char, long> _registers = new();
    private long _ip;

    public Queue<long> Queue { get; } = new();
    public long SendCount { get; private set; }

    public DuetProgram(int id, Instruction[] instructions)
    {
        _instructions = instructions;
        _registers['p'] = id;
    }

    public static long GetValue(Operand operand, Dictionary<char, long> registers)
        => operand.IsRegister ? registers.GetValueOrDefault(operand.Register, 0L) : operand.Literal;

    public bool Step(Queue<long> incoming, Queue<long> outgoing)
    {
        if (_ip < 0 || _ip >= _instructions.Length)
            return false;

        var inst = _instructions[_ip];

        switch (inst.Op)
        {
            case "snd":
                outgoing.Enqueue(GetValue(inst.Arg1, _registers));
                SendCount++;
                _ip++;
                return true;

            case "set":
                _registers[inst.Arg1.Register] = GetValue(inst.Arg2, _registers);
                _ip++;
                return true;

            case "add":
                _registers[inst.Arg1.Register] =
                    _registers.GetValueOrDefault(inst.Arg1.Register, 0L) + GetValue(inst.Arg2, _registers);
                _ip++;
                return true;

            case "mul":
                _registers[inst.Arg1.Register] =
                    _registers.GetValueOrDefault(inst.Arg1.Register, 0L) * GetValue(inst.Arg2, _registers);
                _ip++;
                return true;

            case "mod":
                _registers[inst.Arg1.Register] =
                    _registers.GetValueOrDefault(inst.Arg1.Register, 0L) % GetValue(inst.Arg2, _registers);
                _ip++;
                return true;

            case "rcv":
                if (incoming.Count == 0)
                    return false;

                _registers[inst.Arg1.Register] = incoming.Dequeue();
                _ip++;
                return true;

            case "jgz":
                if (GetValue(inst.Arg1, _registers) > 0)
                    _ip += GetValue(inst.Arg2, _registers);
                else
                    _ip++;
                return true;

            default:
                throw new InvalidOperationException($"Instrucción desconocida: '{inst.Op}'");
        }
    }
}

readonly record struct Operand(bool IsRegister, char Register, long Literal);
readonly record struct Instruction(string Op, Operand Arg1, Operand Arg2);