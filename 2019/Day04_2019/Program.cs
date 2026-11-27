using System;
using System.IO;
using System.Linq;

static class Day04
{
    static bool IsValidPart1(string s)
    {
        bool hasDouble = false;
        for (int i = 0; i < s.Length - 1; i++)
        {
            if (s[i] > s[i + 1]) return false;
            if (s[i] == s[i + 1]) hasDouble = true;
        }
        return hasDouble;
    }

    static bool IsValidPart2(string s)
    {
        for (int i = 0; i < s.Length - 1; i++)
        {
            if (s[i] > s[i + 1]) return false;
        }
        return s.GroupBy(c => c).Any(g => g.Count() == 2);
    }

    public static (int Part1, int Part2) Solve(string raw)
    {
        var parts = raw.Trim().Split('-');
        int low = int.Parse(parts[0]);
        int high = int.Parse(parts[1]);

        int p1 = 0, p2 = 0;
        for (int n = low; n <= high; n++)
        {
            string s = n.ToString();
            if (IsValidPart1(s)) p1++;
            if (IsValidPart2(s)) p2++;
        }
        return (p1, p2);
    }

    static void Main()
    {
        string raw = "123257-647015";
        var (p1, p2) = Solve(raw);
        Console.WriteLine($"Day 4 - Part 1: {p1}, Part 2: {p2}");
    }
}
