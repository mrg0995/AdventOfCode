using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day21
{
    sealed class SpringVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();
        public List<long> Outputs { get; } = new();

        public SpringVm(long[] program, IEnumerable<long> inputs)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
            foreach (var v in inputs) _inputs.Enqueue(v);
        }

        private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0L;
        private void Write(long addr, long val) => _mem[addr] = val;

        private static int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        private long WriteAddr(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return m switch
            {
                0 => raw,
                2 => _relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {m}")
            };
        }

        private long Param(int offset, long modes)
        {
            int m = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return m switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(_relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {m}")
            };
        }

        public List<long> Run()
        {
            while (true)
            {
                long instr = Read(_ip);
                int op = (int)(instr % 100L);
                long modes = instr / 100L;

                if (op == 99) return Outputs;

                switch (op)
                {
                    case 1:
                    case 2:
                    case 7:
                    case 8:
                        {
                            long a = Param(1, modes);
                            long b = Param(2, modes);
                            long dest = WriteAddr(3, modes);
                            Write(dest, op switch
                            {
                                1 => a + b,
                                2 => a * b,
                                7 => a < b ? 1L : 0L,
                                8 => a == b ? 1L : 0L,
                                _ => 0L
                            });
                            _ip += 4;
                            break;
                        }
                    case 3:
                        Write(WriteAddr(1, modes), _inputs.Dequeue());
                        _ip += 2;
                        break;
                    case 4:
                        Outputs.Add(Param(1, modes));
                        _ip += 2;
                        break;
                    case 5:
                    case 6:
                        {
                            long a = Param(1, modes);
                            long b = Param(2, modes);
                            if ((op == 5 && a != 0L) || (op == 6 && a == 0L)) _ip = b;
                            else _ip += 3;
                            break;
                        }
                    case 9:
                        _relBase += Param(1, modes);
                        _ip += 2;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown opcode {op}");
                }
            }
        }
    }

    static long RunScript(long[] program, string[] lines, string cmd)
    {
        string text = string.Join("\n", lines.Concat(new[] { cmd, "" }));
        var inputs = text.Select(ch => (long)ch);
        var outputs = new SpringVm(program, inputs).Run();
        return outputs[^1];
    }

    public static (long Part1, long Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        string[] walk =
        {
            "NOT A J",
            "NOT B T",
            "OR T J",
            "NOT C T",
            "OR T J",
            "AND D J"
        };
        long p1 = RunScript(program, walk, "WALK");

        string[] run =
        {
            "NOT A J",
            "NOT B T",
            "OR T J",
            "NOT C T",
            "OR T J",
            "AND D J",
            "NOT E T",
            "NOT T T",
            "OR H T",
            "AND T J"
        };
        long p2 = RunScript(program, run, "RUN");

        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day21_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 21 - Part 1: {p1}, Part 2: {p2}");
    }
}