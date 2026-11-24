using System;
using System.Collections.Generic;
using System.IO;

class AdventOfCode2016Day23
{
    enum OpCode { Cpy, Inc, Dec, Jnz, Tgl }

    enum OperandKind { Register, Immediate }

    readonly record struct Operand(OperandKind Kind, int Value);

    struct Instruction
    {
        public OpCode Op;
        public Operand Arg1;
        public bool HasArg2;
        public Operand Arg2;
    }

    static void Main()
    {
        string[] lines = File.ReadAllLines("Your_Route");
        var program = ParseProgram(lines);

        int part1 = Run(CloneProgram(program), initialA: 7, optimizeMul: false);
        int part2 = Run(CloneProgram(program), initialA: 12, optimizeMul: true);

        Console.WriteLine($"Parte 1: {part1}");
        Console.WriteLine($"Parte 2: {part2}");
    }

    static Instruction[] ParseProgram(string[] lines)
    {
        var list = new List<Instruction>(lines.Length);

        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            OpCode op = parts[0] switch
            {
                "cpy" => OpCode.Cpy,
                "inc" => OpCode.Inc,
                "dec" => OpCode.Dec,
                "jnz" => OpCode.Jnz,
                "tgl" => OpCode.Tgl,
                _ => throw new InvalidOperationException($"Instrucción desconocida: {parts[0]}")
            };

            Operand arg1 = ParseOperand(parts[1]);
            bool hasArg2 = parts.Length > 2;
            Operand arg2 = hasArg2 ? ParseOperand(parts[2]) : default;

            list.Add(new Instruction
            {
                Op = op,
                Arg1 = arg1,
                HasArg2 = hasArg2,
                Arg2 = arg2
            });
        }

        return list.ToArray();
    }

    static Operand ParseOperand(string token)
    {
        if (token.Length == 1 && token[0] is >= 'a' and <= 'd')
            return new Operand(OperandKind.Register, token[0] - 'a');

        return new Operand(OperandKind.Immediate, int.Parse(token));
    }

    static Instruction[] CloneProgram(Instruction[] source) => (Instruction[])source.Clone();

    static int Run(Instruction[] code, int initialA, bool optimizeMul)
    {
        Span<int> regs = stackalloc int[4];
        regs[0] = initialA;

        int pc = 0;
        int n = code.Length;

        while ((uint)pc < (uint)n)
        {
            if (optimizeMul && pc + 5 < n && TryExecuteMulLoop(code, pc, regs, out int nextPc))
            {
                pc = nextPc;
                continue;
            }

            ref Instruction inst = ref code[pc];

            switch (inst.Op)
            {
                case OpCode.Cpy:
                    if (inst.HasArg2 && inst.Arg2.Kind == OperandKind.Register)
                        regs[inst.Arg2.Value] = Eval(inst.Arg1, regs);
                    pc++;
                    break;

                case OpCode.Inc:
                    if (inst.Arg1.Kind == OperandKind.Register)
                        regs[inst.Arg1.Value]++;
                    pc++;
                    break;

                case OpCode.Dec:
                    if (inst.Arg1.Kind == OperandKind.Register)
                        regs[inst.Arg1.Value]--;
                    pc++;
                    break;

                case OpCode.Jnz:
                    if (!inst.HasArg2)
                    {
                        pc++;
                        break;
                    }
                    int cond = Eval(inst.Arg1, regs);
                    if (cond != 0)
                        pc += Eval(inst.Arg2, regs);
                    else
                        pc++;
                    break;

                case OpCode.Tgl:
                    int target = pc + Eval(inst.Arg1, regs);
                    if ((uint)target < (uint)n)
                        Toggle(ref code[target]);
                    pc++;
                    break;
            }
        }

        return regs[0];
    }

    static int Eval(Operand op, ReadOnlySpan<int> regs) =>
        op.Kind == OperandKind.Register ? regs[op.Value] : op.Value;

    static void Toggle(ref Instruction inst)
    {
        if (!inst.HasArg2)
        {
            inst.Op = inst.Op == OpCode.Inc ? OpCode.Dec : OpCode.Inc;
        }
        else
        {
            inst.Op = inst.Op == OpCode.Jnz ? OpCode.Cpy : OpCode.Jnz;
        }
    }

    static bool TryExecuteMulLoop(Instruction[] code, int pc, Span<int> regs, out int nextPc)
    {
        nextPc = pc;

        ref readonly Instruction i0 = ref code[pc];
        ref readonly Instruction i1 = ref code[pc + 1];
        ref readonly Instruction i2 = ref code[pc + 2];
        ref readonly Instruction i3 = ref code[pc + 3];
        ref readonly Instruction i4 = ref code[pc + 4];
        ref readonly Instruction i5 = ref code[pc + 5];

        if (i0.Op != OpCode.Cpy || !i0.HasArg2 || i0.Arg2.Kind != OperandKind.Register) return false;
        if (i1.Op != OpCode.Inc || i1.Arg1.Kind != OperandKind.Register) return false;
        if (i2.Op != OpCode.Dec || i2.Arg1.Kind != OperandKind.Register) return false;
        if (i3.Op != OpCode.Jnz || !i3.HasArg2 || i3.Arg1.Kind != OperandKind.Register) return false;
        if (i4.Op != OpCode.Dec || i4.Arg1.Kind != OperandKind.Register) return false;
        if (i5.Op != OpCode.Jnz || !i5.HasArg2 || i5.Arg1.Kind != OperandKind.Register) return false;

        int regC = i0.Arg2.Value;
        int regA = i1.Arg1.Value;
        int regD = i4.Arg1.Value;

        if (i2.Arg1.Value != regC) return false;
        if (i3.Arg1.Value != regC || i3.Arg2.Kind != OperandKind.Immediate || i3.Arg2.Value != -2) return false;
        if (i5.Arg1.Value != regD || i5.Arg2.Kind != OperandKind.Immediate || i5.Arg2.Value != -5) return false;

        if (regA == regC || regA == regD || regC == regD) return false;
        if (i0.Arg1.Kind == OperandKind.Register && (i0.Arg1.Value == regA || i0.Arg1.Value == regD)) return false;

        int factor = Eval(i0.Arg1, regs);
        int outerCount = regs[regD];

        if (outerCount > 0)
        {
            regs[regA] += factor * outerCount;
        }
        else if (outerCount < 0)
        {

            return false;
        }

        regs[regC] = 0;
        regs[regD] = 0;
        nextPc = pc + 6;
        return true;
    }
}
