using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        int[] containers = File.ReadAllLines(@"Your_Route")
                               .Select(int.Parse)
                               .ToArray();

        int target = 150;

        List<int> validCombinationsLength = new List<int>();

        void FindCombinations(int index, int currentSum, int count)
        {
            if (currentSum == target)
            {
                validCombinationsLength.Add(count);
                return;
            }

            if (currentSum > target || index >= containers.Length)
            {
                return;
            }

            FindCombinations(index + 1, currentSum + containers[index], count + 1);

            FindCombinations(index + 1, currentSum, count);
        }

        FindCombinations(0, 0, 0);

        int part1 = validCombinationsLength.Count;
        Console.WriteLine($"Parte 1 - Combinaciones totales: {part1}");

        int minContainers = validCombinationsLength.Min();
        int part2 = validCombinationsLength.Count(c => c == minContainers);
        Console.WriteLine($"Parte 2 - Formas usando el mínimo ({minContainers} contenedores): {part2}");
    }
}