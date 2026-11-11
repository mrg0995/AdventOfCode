using System;

int targetRow = 2978;
int targetCol = 3083;

long code = 20151125;
int row = 1;
int col = 1;

while (row != targetRow || col != targetCol)
{
    code = (code * 252533) % 33554393;

    if (row == 1)
    {
        row = col + 1;
        col = 1;
    }
    else
    {
        row--;
        col++;
    }
}

Console.WriteLine($"¡Estrella 50 conseguida! El código en la celda ({targetRow}, {targetCol}) es: {code}");