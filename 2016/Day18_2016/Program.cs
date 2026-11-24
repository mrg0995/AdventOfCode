using System.Diagnostics;

const string puzzleInput = "^^.^..^.....^..^..^^...^^.^....^^^.^.^^....^.^^^...^^^^.^^^^.^..^^^^.^^.^.^.^.^.^^...^^..^^^..^.^^^^";
const int rowsPart1 = 40;
const int rowsPart2 = 400_000;

var sw = Stopwatch.StartNew();

int safeTilesPart1 = CountSafeTiles(puzzleInput.Trim(), rowsPart1);
int safeTilesPart2 = CountSafeTiles(puzzleInput.Trim(), rowsPart2);

sw.Stop();

Console.WriteLine($"Parte 1 ({rowsPart1} filas): {safeTilesPart1} baldosas seguras");
Console.WriteLine($"Parte 2 ({rowsPart2} filas): {safeTilesPart2} baldosas seguras");
Console.WriteLine($"Tiempo total: {sw.ElapsedMilliseconds} ms");

static int CountSafeTiles(string firstRow, int totalRows)
{
    int width = firstRow.Length;
    bool[] currentRow = new bool[width];
    bool[] nextRow = new bool[width];

    int safeCount = 0;

    for (int i = 0; i < width; i++)
    {
        bool isTrap = firstRow[i] == '^';
        currentRow[i] = isTrap;
        if (!isTrap)
        {
            safeCount++;
        }
    }

    for (int row = 1; row < totalRows; row++)
    {
        for (int i = 0; i < width; i++)
        {
            bool leftTrap = i > 0 && currentRow[i - 1];
            bool rightTrap = i < width - 1 && currentRow[i + 1];

            bool isTrap = leftTrap != rightTrap;
            nextRow[i] = isTrap;

            if (!isTrap)
            {
                safeCount++;
            }
        }

        (currentRow, nextRow) = (nextRow, currentRow);
    }

    return safeCount;
}
