using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

string directions = string.Empty;

try
{
    StreamReader sr = new StreamReader("Your_Route");

    directions += sr.ReadLine();

    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

int totalHouses = CountHouses(directions);
Console.WriteLine($"Total de casas visitadas: {totalHouses}");

int totalHouses2 = CountHouses2(directions);
Console.WriteLine($"Total de casas visitadas por Santa y RoboSanta: {totalHouses2}");

static int CountHouses(string directions)
{
    HashSet <(int x, int y)> visited = new HashSet<(int x, int y)>();

    int x = 0;
    int y = 0;

    visited.Add((x,y));

    foreach (char move in directions)
    {
        switch (move)
        {
            case '>':
                x++;
                break;

            case '<':
                x--;
                break;

            case '^':
                y++;
                break;

            case 'v':
                y--;
                break;
        }
        
        visited.Add((x,y));
    }

    return visited.Count();
}

static int CountHouses2(string directions)
{
    HashSet<(int, int)> visited = new HashSet<(int, int)>();

    int Sx = 0;
    int Sy = 0;
    int Rx = 0;
    int Ry = 0;

    visited.Add((0, 0));

    for (int i = 0; i < directions.Length; i++)
    {
        char dir = directions[i];
        int dx = 0, dy = 0;

        switch (dir)
        {
            case '^':
                dy = 1;
                break;

            case 'v':
                dy = -1;
                break;

            case '>':
                dx = 1;
                break;

            case '<':
                dx = -1;
                break;
        }

        if (i % 2 == 0)
        {
            Sx += dx;
            Sy += dy;
            visited.Add((Sx, Sy));
        }

        else
        {
            Rx += dx;
            Ry += dy;
            visited.Add((Rx, Ry));
        }
    }
    return visited.Count;
}

