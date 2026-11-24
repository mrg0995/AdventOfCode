using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

string rutaArchivo = @"Your_Route";
string patron = @"^(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)$";

var ingredientes = File.ReadAllLines(rutaArchivo)
    .Select(line => Regex.Match(line, patron))
    .Where(m => m.Success)
    .Select(m => new Ingrediente(
        Name: m.Groups[1].Value,
        Capacity: int.Parse(m.Groups[2].Value),
        Durability: int.Parse(m.Groups[3].Value),
        Flavor: int.Parse(m.Groups[4].Value),
        Texture: int.Parse(m.Groups[5].Value),
        Calories: int.Parse(m.Groups[6].Value)
    ))
    .ToList();

long maxScorePart1 = 0;
long maxScorePart2 = 0;
int totalSpoons = 100;

if (ingredientes.Count == 4)
{
    for (int i = 0; i <= totalSpoons; i++)
    {
        for (int j = 0; j <= totalSpoons - i; j++)
        {
            for (int k = 0; k <= totalSpoons - i - j; k++)
            {
                int l = totalSpoons - i - j - k;

                var amounts = new[] { i, j, k, l };

                long capacity = Math.Max(0, ingredientes.Select((ing, idx) => (long)ing.Capacity * amounts[idx]).Sum());
                long durability = Math.Max(0, ingredientes.Select((ing, idx) => (long)ing.Durability * amounts[idx]).Sum());
                long flavor = Math.Max(0, ingredientes.Select((ing, idx) => (long)ing.Flavor * amounts[idx]).Sum());
                long texture = Math.Max(0, ingredientes.Select((ing, idx) => (long)ing.Texture * amounts[idx]).Sum());
                long calories = ingredientes.Select((ing, idx) => (long)ing.Calories * amounts[idx]).Sum();
                
                long score = capacity * durability * flavor * texture;

                if (score > maxScorePart1)
                {
                    maxScorePart1 = score;
                }

                if (calories == 500 && score > maxScorePart2)
                {
                    maxScorePart2 = score;
                }
            }
        }
    }
}

Console.WriteLine($"Mejor puntuación total: {maxScorePart1}");
Console.WriteLine($"Mejor puntuación total con 500 calorias: {maxScorePart2}");

record Ingrediente(string Name, int Capacity, int Durability, int Flavor, int Texture, int Calories);
