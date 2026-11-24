using System;
using System.IO;

string[] input = File.ReadAllLines(@"Your_Route");

Console.WriteLine($"Parte 1: {SolveDay18(input, part2: false)}");
Console.WriteLine($"Parte 2: {SolveDay18(input, part2: true)}");

static int SolveDay18(string[] input, bool part2)
{
    int size = input.Length;
    bool[,] grid = new bool[size, size];

    for (int r = 0; r < size; r++)
    {
        for (int c = 0; c < size; c++)
        {
            grid[r, c] = input[r][c] == '#';
        }
    }

    if (part2) SetCorners(grid, size);

    for (int step = 0; step < 100; step++)
    {
        bool[,] nextGrid = new bool[size, size];
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                int neighbors = CountNeighbors(grid, r, c, size);
                if (grid[r, c])
                {
                    nextGrid[r, c] = (neighbors == 2 || neighbors == 3);
                }
                else
                {
                    nextGrid[r, c] = (neighbors == 3);
                }
            }
        }

        if (part2) SetCorners(nextGrid, size);
        grid = nextGrid;
    }

    int count = 0;
    for (int r = 0; r < size; r++)
    {
        for (int c = 0; c < size; c++)
        {
            if (grid[r, c]) count++;
        }
    }
    return count;
}

static void SetCorners(bool[,] grid, int size)
{
    grid[0, 0] = true;
    grid[0, size - 1] = true;
    grid[size - 1, 0] = true;
    grid[size - 1, size - 1] = true;
}

static int CountNeighbors(bool[,] grid, int r, int c, int size)
{
    int count = 0;
    for (int dr = -1; dr <= 1; dr++)
    {
        for (int dch = -1; dch <= 1; dch++)
        {
            if (dr == 0 && dch == 0) continue;
            int nr = r + dr;
            int nc = c + dch;
            if (nr >= 0 && nr < size && nc >= 0 && nc < size)
            {
                if (grid[nr, nc]) count++;
            }
        }
    }
    return count;
}
