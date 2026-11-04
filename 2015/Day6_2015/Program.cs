using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

string line = string.Empty;
List<string> coordenadas = new List<string>();

try
{
    StreamReader sr = new StreamReader("Your_Route");

    line = sr.ReadLine();

    while (line != null && !string.IsNullOrWhiteSpace(line))
    {
        coordenadas.Add(line);
        line = sr.ReadLine();
    }

    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

int[,] grid = new int[1000, 1000];

string pattern = @"(turn on|turn off|toggle)\s+(\d+),(\d+)\s+through\s+(\d+),(\d+)";
Regex regex = new Regex(pattern);

foreach (string instruccion in coordenadas)
{
    Match match = regex.Match(instruccion);
    if (match.Success)
    {
        string accion = match.Groups[1].Value;
        int x1 = int.Parse(match.Groups[2].Value);
        int y1 = int.Parse(match.Groups[3].Value);
        int x2 = int.Parse(match.Groups[4].Value);
        int y2 = int.Parse(match.Groups[5].Value);

        ApplyInstruction(grid, accion, x1, y1, x2, y2);
    }
}

int lightsOn = CountingLights(grid);
Console.WriteLine($"Total de luces encendidas al final: {lightsOn}");

long totalBrightness = TotalBrightness(grid);
Console.WriteLine($"Brillo total de todas las luces: {totalBrightness}");

static void ApplyInstruction(int[,] grid, string accion, int x1, int y1, int x2, int y2)
{
    for (int x = x1; x <= x2; x++)
    {
        for (int y = y1; y <= y2; y++)
        {
            if (accion == "turn on")
            {
                grid[x, y] += 1;
            }
            else if (accion == "turn off")
            {
                grid[x, y] = Math.Max(0, grid[x, y] - 1);
            }
            else if (accion == "toggle")
            {
                grid[x, y] += 2;
            }
        }
    }
}

static int CountingLights(int[,] grid)
{
    int contador = 0;
    int filas = grid.GetLength(0);
    int columnas = grid.GetLength(1);

    for (int x = 0; x < filas; x++)
    {
        for (int y = 0; y < columnas; y++)
        {
            if (grid[x, y] == 1)
            {
                contador++;
            }
        }
    }

    return contador;
}

static long TotalBrightness(int[,] grid)
{
    long sumaTotal = 0;
    int filas = grid.GetLength(0);
    int columnas = grid.GetLength(1);

    for (int x = 0; x < filas; x++)
    {
        for (int y = 0; y < columnas; y++)
        {
            sumaTotal += grid[x, y];
        }
    }

    return sumaTotal;
}