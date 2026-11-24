using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

string[] instructions = File.ReadAllLines("Your_Route");

string part1 = Scramble("abcdefgh", instructions);
string part2 = Unscramble("fbgdceah", instructions);

Console.WriteLine($"Parte 1: {part1}");
Console.WriteLine($"Parte 2: {part2}");


static string Scramble(string input, string[] instructions)
{
var chars = input.ToCharArray();
int len = chars.Length;

foreach (string line in instructions)
{
if (string.IsNullOrWhiteSpace(line)) continue;
string[] parts = line.Split(' ');

if (line.StartsWith("swap position "))
{
int x = int.Parse(parts[2]);
int y = int.Parse(parts[5]);
(chars[x], chars[y]) = (chars[y], chars[x]);
}
else if (line.StartsWith("swap letter "))
{
char a = parts[2][0];
char b = parts[5][0];
for (int i = 0; i < len; i++)
{
if (chars[i] == a) chars[i] = b;
else if (chars[i] == b) chars[i] = a;
}
}
else if (line.StartsWith("rotate left "))
{
int steps = int.Parse(parts[2]) % len;
RotateLeft(chars, steps);
}
else if (line.StartsWith("rotate right "))
{
int steps = int.Parse(parts[2]) % len;
RotateRight(chars, steps);
}
else if (line.StartsWith("rotate based on position of letter "))
{
char target = parts[6][0];
int idx = Array.IndexOf(chars, target);
int steps = 1 + idx + (idx >= 4 ? 1 : 0);
RotateRight(chars, steps % len);
}
else if (line.StartsWith("reverse positions "))
{
int x = int.Parse(parts[2]);
int y = int.Parse(parts[4]);
Array.Reverse(chars, x, y - x + 1);
}
else if (line.StartsWith("move position "))
{
int x = int.Parse(parts[2]);
int y = int.Parse(parts[5]);
var list = new List<char>(chars);
char c = list[x];
list.RemoveAt(x);
list.Insert(y, c);
chars = list.ToArray();
}
else
{
throw new InvalidOperationException($"Instrucción no reconocida: {line}");
}
}

return new string(chars);
}

static string Unscramble(string targetScrambled, string[] instructions)
{
char[] seed = targetScrambled.ToCharArray();
Array.Sort(seed);

foreach (string candidate in GetPermutations(seed))
{
if (Scramble(candidate, instructions) == targetScrambled)
{
return candidate;
}
}

throw new InvalidOperationException("No se encontró ninguna permutación válida.");
}

static void RotateLeft(char[] arr, int steps)
{
int len = arr.Length;
steps %= len;
if (steps == 0) return;

Array.Reverse(arr, 0, steps);
Array.Reverse(arr, steps, len - steps);
Array.Reverse(arr, 0, len);
}

static void RotateRight(char[] arr, int steps)
{
int len = arr.Length;
steps %= len;
if (steps == 0) return;

RotateLeft(arr, len - steps);
}

static IEnumerable<string> GetPermutations(char[] arr)
{
    int n = arr.Length;
    int[] indices = new int[n];
    char[] current = (char[])arr.Clone();

    yield return new string(current);

    int i = 0;
    while (i < n)
    {
        if (indices[i] < i)
        {
            int swapIdx = (i % 2 == 0) ? 0 : indices[i];
            (current[swapIdx], current[i]) = (current[i], current[swapIdx]);
            yield return new string(current);
            indices[i]++;
            i = 0;
        }
        else
        {
            indices[i] = 0;
            i++;
        }
    }
}
