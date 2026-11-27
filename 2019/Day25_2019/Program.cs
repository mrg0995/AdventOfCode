using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

static class Day25
{
    sealed class TextAdventureVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();
        public bool Halted { get; private set; }

        public TextAdventureVm(long[] program)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
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

        public string SendCommand(string cmd)
        {
            foreach (char ch in cmd + "\n") _inputs.Enqueue(ch);
            return RunUntilPrompt();
        }

        public string RunUntilPrompt()
        {
            var sb = new StringBuilder();
            while (true)
            {
                long instr = Read(_ip);
                int op = (int)(instr % 100L);
                long modes = instr / 100L;

                if (op == 99)
                {
                    Halted = true;
                    return sb.ToString();
                }

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
                        if (_inputs.Count == 0)
                        {
                            string curText = sb.ToString();
                            if (curText.EndsWith("Command?\n")) return curText;
                        }
                        Write(WriteAddr(1, modes), _inputs.Dequeue());
                        _ip += 2;
                        break;
                    case 4:
                        {
                            long val = Param(1, modes);
                            _ip += 2;
                            sb.Append(val < 128 ? (char)val : $"[{val}]");
                            break;
                        }
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

    static readonly HashSet<string> UnsafeItems = new()
    {
        "infinite loop",
        "molten lava",
        "giant electromagnet",
        "escape pod",
        "photons"
    };

    static readonly Dictionary<string, string> Opposite = new()
    {
        ["north"] = "south",
        ["south"] = "north",
        ["east"] = "west",
        ["west"] = "east"
    };

    static (string room, List<string> doors, List<string> items) ParseRoom(string output)
    {
        var roomMatches = Regex.Matches(output, @"== ([^=]+) ==");
        string room = roomMatches.Count > 0 ? roomMatches[^1].Groups[1].Value.Trim() : "";

        var doors = new List<string>();
        var items = new List<string>();
        string? mode = null;

        foreach (var rawLine in output.Split('\n'))
        {
            string s = rawLine.Trim();
            if (s == "Doors here lead:") { mode = "doors"; continue; }
            if (s == "Items here:") { mode = "items"; continue; }
            if (s.StartsWith("Command?") || s.Length == 0) { mode = null; continue; }
            if (s.StartsWith("- "))
            {
                string val = s[2..].Trim();
                if (mode == "doors") doors.Add(val);
                else if (mode == "items") items.Add(val);
            }
        }

        return (room, doors, items);
    }

    static IEnumerable<List<T>> SubsetsNonEmpty<T>(List<T> items)
    {
        int total = 1 << items.Count;
        for (int mask = 1; mask < total; mask++)
        {
            var subset = new List<T>();
            for (int i = 0; i < items.Count; i++)
            {
                if ((mask & (1 << i)) != 0) subset.Add(items[i]);
            }
            yield return subset;
        }
    }

    static string SolveDay25(long[] program)
    {
        var vm = new TextAdventureVm(program);
        string intro = vm.RunUntilPrompt();
        var (r0, d0, it0) = ParseRoom(intro);

        var held = new HashSet<string>();
        var visited = new HashSet<(string room, string invKey)>();
        string? floorDir = null;

        bool Explore(string room, List<string> doors, List<string> items, string? cameFrom)
        {
            foreach (var it in items)
            {
                if (!UnsafeItems.Contains(it) && held.Add(it))
                {
                    vm.SendCommand($"take {it}");
                }
            }

            if (room == "Security Checkpoint")
            {
                foreach (var d in doors)
                {
                    if (cameFrom is null || d != Opposite[cameFrom])
                    {
                        floorDir = d;
                    }
                }
                return true;
            }

            string invKey = string.Join("|", held.OrderBy(x => x));
            if (!visited.Add((room, invKey))) return false;

            foreach (var d in doors)
            {
                if (cameFrom != null && d == Opposite[cameFrom]) continue;
                string nxtOut = vm.SendCommand(d);
                var (nr, nd, nit) = ParseRoom(nxtOut);
                if (string.IsNullOrEmpty(nr)) continue;

                if (Explore(nr, nd, nit, d)) return true;
                vm.SendCommand(Opposite[d]);
            }

            return false;
        }

        Explore(r0, d0, it0, null);

        var allHeld = held.OrderBy(x => x).ToList();
        foreach (var it in allHeld) vm.SendCommand($"drop {it}");
        var currentlyHeld = new HashSet<string>();

        string dirToFloor = floorDir ?? "north";
        var codeRx = new Regex(@"typing (\d+) on the keypad|(\d{6,})");

        foreach (var comb in SubsetsNonEmpty(allHeld))
        {
            var target = new HashSet<string>(comb);
            foreach (var it in currentlyHeld.Except(target).ToList())
            {
                vm.SendCommand($"drop {it}");
                currentlyHeld.Remove(it);
            }
            foreach (var it in target.Except(currentlyHeld).ToList())
            {
                vm.SendCommand($"take {it}");
                currentlyHeld.Add(it);
            }

            string attempt = vm.SendCommand(dirToFloor);
            var m = codeRx.Match(attempt);
            if (m.Success)
            {
                return !string.IsNullOrEmpty(m.Groups[1].Value) ? m.Groups[1].Value : m.Groups[2].Value;
            }
        }

        return "Code not found";
    }

    public static (string Part1, string Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        string p1 = SolveDay25(program);
        string p2 = "Complete all 49 other stars to trigger Part 2";
        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day25_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 25 - Part 1: {p1}\nDay 25 - Part 2: {p2}");
    }
}