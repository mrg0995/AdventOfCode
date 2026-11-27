using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

static class Day17
{
    sealed class AsciiVm
    {
        private readonly Dictionary<long, long> _mem = new();
        private long _ip, _relBase;
        private readonly Queue<long> _inputs = new();
        public List<long> Outputs { get; } = new();

        public AsciiVm(long[] program, IEnumerable<long>? inputs = null)
        {
            for (int i = 0; i < program.Length; i++) _mem[i] = program[i];
            if (inputs != null)
            {
                foreach (var v in inputs) _inputs.Enqueue(v);
            }
        }

        private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0L;
        private void Write(long addr, long v) => _mem[addr] = v;

        private static int Mode(int offset, long modes)
        {
            long div = offset switch { 1 => 1L, 2 => 10L, 3 => 100L, _ => 1L };
            return (int)((modes / div) % 10L);
        }

        private long WriteAddr(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => raw,
                2 => _relBase + raw,
                _ => throw new InvalidOperationException($"Invalid write mode {mode}")
            };
        }

        private long Param(int offset, long modes)
        {
            int mode = Mode(offset, modes);
            long raw = Read(_ip + offset);
            return mode switch
            {
                0 => Read(raw),
                1 => raw,
                2 => Read(_relBase + raw),
                _ => throw new InvalidOperationException($"Unknown mode {mode}")
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

    static List<string> BuildFullPath(Dictionary<(int x, int y), char> grid, (int x, int y) start, char initDir)
    {
        (int dx, int dy)[] dirs = { (0, -1), (1, 0), (0, 1), (-1, 0) };
        char[] order = { '^', '>', 'v', '<' };
        int dIdx = Array.IndexOf(order, initDir);
        int x = start.x, y = start.y;
        var tokens = new List<string>();

        while (true)
        {
            var (dx, dy) = dirs[dIdx];
            if (grid.TryGetValue((x + dx, y + dy), out var ahead) && ahead == '#')
            {
                int steps = 0;
                while (grid.TryGetValue((x + dx, y + dy), out var c) && c == '#')
                {
                    x += dx;
                    y += dy;
                    steps++;
                }
                tokens.Add(steps.ToString());
                continue;
            }

            int leftIdx = (dIdx + 3) % 4;
            int rightIdx = (dIdx + 1) % 4;
            if (grid.TryGetValue((x + dirs[leftIdx].dx, y + dirs[leftIdx].dy), out var lc) && lc == '#')
            {
                tokens.Add("L");
                dIdx = leftIdx;
            }
            else if (grid.TryGetValue((x + dirs[rightIdx].dx, y + dirs[rightIdx].dy), out var rc) && rc == '#')
            {
                tokens.Add("R");
                dIdx = rightIdx;
            }
            else
            {
                break;
            }
        }

        return tokens;
    }

    static string SliceToStr(List<(string d, string s)> moves, int start, int len)
    {
        var sb = new StringBuilder();
        for (int i = start; i < start + len; i++)
        {
            if (i > start) sb.Append(',');
            sb.Append(moves[i].d).Append(',').Append(moves[i].s);
        }
        return sb.ToString();
    }

    static string? Encode(List<(string d, string s)> moves, string aS, string bS, string cS)
    {
        int aLen = string.IsNullOrEmpty(aS) ? 0 : aS.Split(',').Length / 2;
        int bLen = string.IsNullOrEmpty(bS) ? 0 : bS.Split(',').Length / 2;
        int cLen = string.IsNullOrEmpty(cS) ? 0 : cS.Split(',').Length / 2;

        var mainParts = new List<string>();
        int i = 0;
        while (i < moves.Count)
        {
            if (aLen > 0 && i + aLen <= moves.Count && SliceToStr(moves, i, aLen) == aS)
            {
                mainParts.Add("A");
                i += aLen;
            }
            else if (bLen > 0 && i + bLen <= moves.Count && SliceToStr(moves, i, bLen) == bS)
            {
                mainParts.Add("B");
                i += bLen;
            }
            else if (cLen > 0 && i + cLen <= moves.Count && SliceToStr(moves, i, cLen) == cS)
            {
                mainParts.Add("C");
                i += cLen;
            }
            else
            {
                return null;
            }
        }

        string main = string.Join(",", mainParts);
        return main.Length <= 20 ? main : null;
    }

    static (string main, string a, string b, string c)? CompressMovement(List<string> tokens)
    {
        var moves = new List<(string d, string s)>();
        for (int i = 0; i + 1 < tokens.Count; i += 2) moves.Add((tokens[i], tokens[i + 1]));

        for (int aLen = 1; aLen <= Math.Min(6, moves.Count); aLen++)
        {
            string aS = SliceToStr(moves, 0, aLen);
            if (aS.Length > 20) break;

            int idxAfterA = 0;
            while (idxAfterA + aLen <= moves.Count && SliceToStr(moves, idxAfterA, aLen) == aS)
                idxAfterA += aLen;

            for (int bLen = 1; bLen <= Math.Min(6, moves.Count - idxAfterA); bLen++)
            {
                string bS = SliceToStr(moves, idxAfterA, bLen);
                if (bS.Length > 20) break;

                int i = 0;
                while (i < moves.Count)
                {
                    if (i + aLen <= moves.Count && SliceToStr(moves, i, aLen) == aS) i += aLen;
                    else if (i + bLen <= moves.Count && SliceToStr(moves, i, bLen) == bS) i += bLen;
                    else break;
                }

                if (i >= moves.Count)
                {
                    var m0 = Encode(moves, aS, bS, "");
                    if (m0 != null) return (m0, aS, bS, "");
                    continue;
                }

                for (int cLen = 1; cLen <= Math.Min(6, moves.Count - i); cLen++)
                {
                    string cS = SliceToStr(moves, i, cLen);
                    if (cS.Length > 20) break;

                    var m = Encode(moves, aS, bS, cS);
                    if (m != null) return (m, aS, bS, cS);
                }
            }
        }

        return null;
    }

    public static (int Part1, long Part2) Solve(string raw)
    {
        long[] program = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        var out1 = new AsciiVm(program).Run();
        string text = new string(out1.Where(c => c < 128).Select(c => (char)c).ToArray());

        var grid = new Dictionary<(int x, int y), char>();
        (int x, int y) robotPos = (0, 0);
        char robotDir = '^';

        var rows = text.Split('\n');
        for (int y = 0; y < rows.Length; y++)
        {
            string row = rows[y].TrimEnd('\r');
            for (int x = 0; x < row.Length; x++)
            {
                char ch = row[x];
                grid[(x, y)] = ch;
                if (ch is '^' or '>' or 'v' or '<')
                {
                    robotPos = (x, y);
                    robotDir = ch;
                }
            }
        }

        (int dx, int dy)[] adj = { (0, -1), (0, 1), (-1, 0), (1, 0) };
        int alignmentSum = 0;
        foreach (var ((x, y), ch) in grid)
        {
            if (ch == '#' && adj.All(d => grid.TryGetValue((x + d.dx, y + d.dy), out var nc) && nc == '#'))
            {
                alignmentSum += x * y;
            }
        }

        var tokens = BuildFullPath(grid, robotPos, robotDir);
        var comp = CompressMovement(tokens) ?? throw new InvalidOperationException("Failed to compress routine");

        long[] prog2 = (long[])program.Clone();
        prog2[0] = 2L;
        string script = $"{comp.main}\n{comp.a}\n{comp.b}\n{comp.c}\nn\n";
        var inputs = script.Select(ch => (long)ch);
        var out2 = new AsciiVm(prog2, inputs).Run();

        return (alignmentSum, out2[^1]);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day17_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 17 - Part 1: {p1}, Part 2: {p2}");
    }
}