using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

static class Day12
{
    static void StepAxis(int[] pos, int[] vel)
    {
        int n = pos.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (pos[i] < pos[j]) { vel[i]++; vel[j]--; }
                else if (pos[i] > pos[j]) { vel[i]--; vel[j]++; }
            }
        }
        for (int i = 0; i < n; i++) pos[i] += vel[i];
    }

    static long AxisCycle(int[] initPos)
    {
        int[] pos = (int[])initPos.Clone();
        int[] vel = new int[pos.Length];
        long steps = 0;

        while (true)
        {
            StepAxis(pos, vel);
            steps++;
            bool samePos = true;
            for (int i = 0; i < pos.Length; i++)
            {
                if (pos[i] != initPos[i] || vel[i] != 0) { samePos = false; break; }
            }
            if (samePos) return steps;
        }
    }

    static long Gcd(long a, long b)
    {
        while (b != 0) { long t = a % b; a = b; b = t; }
        return Math.Abs(a);
    }

    static long Lcm(long a, long b) => (a / Gcd(a, b)) * b;

    public static (int Part1, long Part2) Solve(string[] lines)
    {
        var moons = new List<(int x, int y, int z)>();
        var rx = new Regex(@"-?\d+");

        foreach (var line in lines)
        {
            var matches = rx.Matches(line);
            if (matches.Count == 3)
            {
                moons.Add((
                    int.Parse(matches[0].Value),
                    int.Parse(matches[1].Value),
                    int.Parse(matches[2].Value)
                ));
            }
        }

        int n = moons.Count;
        int[] px = new int[n], py = new int[n], pz = new int[n];
        int[] vx = new int[n], vy = new int[n], vz = new int[n];
        for (int i = 0; i < n; i++) { px[i] = moons[i].x; py[i] = moons[i].y; pz[i] = moons[i].z; }

        for (int s = 0; s < 1000; s++)
        {
            StepAxis(px, vx);
            StepAxis(py, vy);
            StepAxis(pz, vz);
        }

        int p1 = 0;
        for (int i = 0; i < n; i++)
        {
            int pot = Math.Abs(px[i]) + Math.Abs(py[i]) + Math.Abs(pz[i]);
            int kin = Math.Abs(vx[i]) + Math.Abs(vy[i]) + Math.Abs(vz[i]);
            p1 += pot * kin;
        }

        long cx = AxisCycle(moons.ConvertAll(m => m.x).ToArray());
        long cy = AxisCycle(moons.ConvertAll(m => m.y).ToArray());
        long cz = AxisCycle(moons.ConvertAll(m => m.z).ToArray());
        long p2 = Lcm(Lcm(cx, cy), cz);

        return (p1, p2);
    }

    static void Main()
    {
        var lines = File.ReadAllLines("Day12_2019.txt");
        var (p1, p2) = Solve(lines);
        Console.WriteLine($"Day 12 - Part 1: {p1}, Part 2: {p2}");
    }
}