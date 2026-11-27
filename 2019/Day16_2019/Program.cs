using System;
using System.IO;
using System.Linq;

static class Day16
{
    static readonly int[] BasePattern = { 0, 1, 0, -1 };

    static int[] FftPhasePart1(int[] signal)
    {
        int n = signal.Length;
        int[] outArr = new int[n];
        for (int i = 0; i < n; i++)
        {
            int rep = i + 1;
            int total = 0;
            for (int j = 0; j < n; j++)
            {
                int coeff = BasePattern[((j + 1) / rep) % 4];
                if (coeff != 0) total += signal[j] * coeff;
            }
            outArr[i] = Math.Abs(total) % 10;
        }
        return outArr;
    }

    static int[] FftPhaseSecondHalf(int[] tail)
    {
        int running = 0;
        int[] outArr = new int[tail.Length];
        for (int i = tail.Length - 1; i >= 0; i--)
        {
            running = (running + tail[i]) % 10;
            outArr[i] = running;
        }
        return outArr;
    }

    public static (string Part1, string Part2) Solve(string raw)
    {
        int[] digits = raw.Trim().Where(char.IsDigit).Select(c => c - '0').ToArray();

        int[] sig1 = (int[])digits.Clone();
        for (int phase = 0; phase < 100; phase++)
        {
            sig1 = FftPhasePart1(sig1);
        }
        string p1 = string.Concat(sig1.Take(8));

        int offset = int.Parse(string.Concat(digits.Take(7)));
        int totalLen = digits.Length * 10_000;
        int[] tail = new int[totalLen - offset];
        for (int idx = offset; idx < totalLen; idx++)
        {
            tail[idx - offset] = digits[idx % digits.Length];
        }

        for (int phase = 0; phase < 100; phase++)
        {
            tail = FftPhaseSecondHalf(tail);
        }
        string p2 = string.Concat(tail.Take(8));

        return (p1, p2);
    }

    static void Main()
    {
        string raw = File.ReadAllText("Day16_2019.txt");
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 16 - Part 1: {p1}, Part 2: {p2}");
    }
}