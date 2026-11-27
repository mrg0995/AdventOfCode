using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

static class Day07
{
    sealed class IntcodeVm
    {
        private readonly int[] _mem;
        private int _ip;
        private readonly Queue<int> _inputs = new();

        public List<int> Outputs { get; } = new();
        public bool Halted { get; private set; }

        public IntcodeVm(int[] program, IEnumerable<int>? initialInputs = null)
        {
            _mem = (int[])program.Clone();
            if (initialInputs != null)
            {
                foreach (var v in initialInputs) _inputs.Enqueue(v);
            }
        }

        public void EnqueueInput(int value) => _inputs.Enqueue(value);

        private int GetParam(int offset, int modes)
        {
            int divisor = offset switch { 1 => 1, 2 => 10, 3 => 100, _ => 1 };
            int mode = (modes / divisor) % 10;
            int val = _mem[_ip + offset];
            return mode == 0 ? _mem[val] : val;
        }

        public void RunUntilPauseOrHalt()
        {
            while (_ip < _mem.Length)
            {
                int instr = _mem[_ip];
                int op = instr % 100;
                int modes = instr / 100;

                if (op == 99)
                {
                    Halted = true;
                    return;
                }

                switch (op)
                {
                    case 1:
                    case 2:
                    case 7:
                    case 8:
                        {
                            int a = GetParam(1, modes);
                            int b = GetParam(2, modes);
                            int dest = _mem[_ip + 3];
                            _mem[dest] = op switch
                            {
                                1 => a + b,
                                2 => a * b,
                                7 => a < b ? 1 : 0,
                                8 => a == b ? 1 : 0,
                                _ => 0
                            };
                            _ip += 4;
                            break;
                        }
                    case 3:
                        if (_inputs.Count == 0) return;
                        _mem[_mem[_ip + 1]] = _inputs.Dequeue();
                        _ip += 2;
                        break;
                    case 4:
                        Outputs.Add(GetParam(1, modes));
                        _ip += 2;
                        break;
                    case 5:
                    case 6:
                        {
                            int a = GetParam(1, modes);
                            int b = GetParam(2, modes);
                            if ((op == 5 && a != 0) || (op == 6 && a == 0)) _ip = b;
                            else _ip += 3;
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Unknown opcode {op} at {_ip}");
                }
            }
        }
    }

    static IEnumerable<int[]> Permutations(int[] items)
    {
        int n = items.Length;
        var a = (int[])items.Clone();
        var c = new int[n];
        yield return (int[])a.Clone();

        int i = 0;
        while (i < n)
        {
            if (c[i] < i)
            {
                if (i % 2 == 0) (a[0], a[i]) = (a[i], a[0]);
                else (a[c[i]], a[i]) = (a[i], a[c[i]]);
                yield return (int[])a.Clone();
                c[i]++;
                i = 0;
            }
            else
            {
                c[i] = 0;
                i++;
            }
        }
    }

    static int EvalSeries(int[] program, int[] phases)
    {
        int signal = 0;
        foreach (int phase in phases)
        {
            var vm = new IntcodeVm(program, new[] { phase, signal });
            vm.RunUntilPauseOrHalt();
            signal = vm.Outputs[^1];
        }
        return signal;
    }

    static int EvalLoop(int[] program, int[] phases)
    {
        var amps = phases.Select(p => new IntcodeVm(program, new[] { p })).ToArray();
        int signal = 0, idx = 0, lastESignal = 0;

        while (!amps[^1].Halted)
        {
            var vm = amps[idx];
            vm.EnqueueInput(signal);
            vm.RunUntilPauseOrHalt();
            if (vm.Outputs.Count > 0)
            {
                signal = vm.Outputs[^1];
                if (idx == amps.Length - 1) lastESignal = signal;
            }
            idx = (idx + 1) % amps.Length;
        }

        return lastESignal;
    }

    public static (int Part1, int Part2) Solve(string raw)
    {
        int[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

        int p1 = Permutations(new[] { 0, 1, 2, 3, 4 }).Max(p => EvalSeries(program, p));
        int p2 = Permutations(new[] { 5, 6, 7, 8, 9 }).Max(p => EvalLoop(program, p));
        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day07_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 7 - Part 1: {p1}, Part 2: {p2}");
    }
}