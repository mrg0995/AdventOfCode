using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

const string passcode = "bwnlcvfs";

var sw = Stopwatch.StartNew();
var (shortestPath, longestPathLength) = Solve(passcode);
sw.Stop();

Console.WriteLine($"Parte 1 (camino más corto): {shortestPath}");
Console.WriteLine($"Parte 2 (longitud máxima):  {longestPathLength}");
Console.WriteLine($"Tiempo total: {sw.ElapsedMilliseconds} ms");

static (string ShortestPath, int LongestLength) Solve(string passcode)
{
    (char Dir, int Dx, int Dy)[] directions =
    [
        ('U',  0, -1),
        ('D',  0,  1),
        ('L', -1,  0),
        ('R',  1,  0)
    ];

    var queue = new Queue<(int X, int Y, string Path)>();
    queue.Enqueue((0, 0, ""));

    string? shortestPath = null;
    int longestLength = 0;

    while (queue.Count > 0)
    {
        var (x, y, path) = queue.Dequeue();

        if (x == 3 && y == 3)
        {
            shortestPath ??= path;
            if (path.Length > longestLength)
            {
                longestLength = path.Length;
            }
            continue;
        }

        string hash = ComputeMd5Hex(passcode + path);

        for (int i = 0; i < 4; i++)
        {
            if (hash[i] is >= 'b' and <= 'f')
            {
                int nx = x + directions[i].Dx;
                int ny = y + directions[i].Dy;

                if (nx is >= 0 and < 4 && ny is >= 0 and < 4)
                {
                    queue.Enqueue((nx, ny, path + directions[i].Dir));
                }
            }
        }
    }

    return (shortestPath ?? string.Empty, longestLength);
}

static string ComputeMd5Hex(string input)
{
    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
    byte[] hashBytes = MD5.HashData(inputBytes);
    return Convert.ToHexStringLower(hashBytes);
}
