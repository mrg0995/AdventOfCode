using System;
using System.IO;

string path = @"Your_Route";
string input = File.ReadAllText(path);

Console.WriteLine($"Parte 1 - Código del baño: {SolvePart1(input)}");
Console.WriteLine($"Parte 2 - Código avanzado: {SolvePart2(input)}");

static string SolvePart1(string input)
{
    char[,] keypad = {
        { '1', '2', '3' },
        { '4', '5', '6' },
        { '7', '8', '9' }
    };

    int row = 1, col = 1;
    string code = "";

    foreach (var line in input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
    {
        foreach (char move in line.Trim())
        {
            int newRow = row, newCol = col;

            if (move == 'U') newRow--;
            else if (move == 'D') newRow++;
            else if (move == 'L') newCol--;
            else if (move == 'R') newCol++;

            if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
            {
                row = newRow;
                col = newCol;
            }
        }
        code += keypad[row, col];
    }

    return code;
}

static string SolvePart2(string input)
{
    char[,] keypad = {
        { ' ', ' ', '1', ' ', ' ' },
        { ' ', '2', '3', '4', ' ' },
        { '5', '6', '7', '8', '9' },
        { ' ', 'A', 'B', 'C', ' ' },
        { ' ', ' ', 'D', ' ', ' ' }
    };

    int row = 2, col = 0;
    string code = "";

    foreach (var line in input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
    {
        foreach (char move in line.Trim())
        {
            int newRow = row, newCol = col;

            if (move == 'U') newRow--;
            else if (move == 'D') newRow++;
            else if (move == 'L') newCol--;
            else if (move == 'R') newCol++;

            if (newRow >= 0 && newRow < 5 && newCol >= 0 && newCol < 5 && keypad[newRow, newCol] != ' ')
            {
                row = newRow;
                col = newCol;
            }
        }
        code += keypad[row, col];
    }

    return code;
}