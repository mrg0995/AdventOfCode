using System;
using System.Collections.Generic;
using System.Linq;

static class Day06
{
    public static (int Part1, int Part2) Solve(string input)
    {
        int[] banks = input
            .Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        var seen = new Dictionary<string, int>();
        int cycles = 0;

        while (true)
        {
            string key = string.Join(",", banks);
            if (seen.TryGetValue(key, out int firstSeenCycle))
            {
                return (cycles, cycles - firstSeenCycle);
            }

            seen[key] = cycles;

            int maxIndex = 0;
            for (int i = 1; i < banks.Length; i++)
            {
                if (banks[i] > banks[maxIndex])
                    maxIndex = i;
            }

            int blocks = banks[maxIndex];
            banks[maxIndex] = 0;

            int idx = (maxIndex + 1) % banks.Length;
            while (blocks > 0)
            {
                banks[idx]++;
                blocks--;
                idx = (idx + 1) % banks.Length;
            }

            cycles++;
        }
    }
}
