using System;
using System.IO;
using System.Linq;

int[][] rows = File.ReadAllLines("input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(line => line
        .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToArray())
    .ToArray();

int part1 = rows.Sum(RowDifference);
int part2 = rows.Sum(RowEvenlyDivisibleQuotient);

Console.WriteLine($"Parte 1: {part1}");
Console.WriteLine($"Parte 2: {part2}");

static int RowDifference(int[] row)
{
    int min = row.Min();
    int max = row.Max();
    return max - min;
}

static int RowEvenlyDivisibleQuotient(int[] row)
{
    for (int i = 0; i < row.Length; i++)
    {
        for (int j = 0; j < row.Length; j++)
        {
            if (i == j)
            {
                continue;
            }

            if (row[i] % row[j] == 0)
            {
                return row[i] / row[j];
            }
        }
    }

    throw new InvalidOperationException("No se encontró ningún par divisible de forma exacta en la fila.");
}