using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

const int Width = 50;
const int Height = 6;

string[] lines = File.ReadAllLines("Your_Route");
bool[,] screen = new bool[Height, Width];

foreach (var rawLine in lines)
{
    string line = rawLine.Trim();
    if (string.IsNullOrEmpty(line)) continue;

    var matches = Regex.Matches(line, @"\d+");
    int a = int.Parse(matches[0].Value);
    int b = int.Parse(matches[1].Value);

    if (line.StartsWith("rect"))
    {
        for (int r = 0; r < b; r++)
        {
            for (int c = 0; c < a; c++)
            {
                screen[r, c] = true;
            }
        }
    }
    else if (line.StartsWith("rotate row"))
    {
        int y = a;
        int shift = b % Width;
        bool[] newRow = new bool[Width];

        for (int c = 0; c < Width; c++)
        {
            newRow[(c + shift) % Width] = screen[y, c];
        }

        for (int c = 0; c < Width; c++)
        {
            screen[y, c] = newRow[c];
        }
    }
    else if (line.StartsWith("rotate column"))
    {
        int x = a;
        int shift = b % Height;
        bool[] newCol = new bool[Height];

        for (int r = 0; r < Height; r++)
        {
            newCol[(r + shift) % Height] = screen[r, x];
        }

        for (int r = 0; r < Height; r++)
        {
            screen[r, x] = newCol[r];
        }
    }
}

int litPixels = 0;
for (int r = 0; r < Height; r++)
{
    for (int c = 0; c < Width; c++)
    {
        if (screen[r, c]) litPixels++;
    }
}

Console.WriteLine($"Resultado Parte 1 (Píxeles encendidos): {litPixels}\n");

Console.WriteLine("Resultado Parte 2 (Pantalla):");
for (int r = 0; r < Height; r++)
{
    for (int c = 0; c < Width; c++)
    {
        Console.Write(screen[r, c] ? '#' : ' ');
    }
    Console.WriteLine();
}