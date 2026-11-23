using System;
using System.IO;
using System.Linq;

string input = File.ReadAllText("input.txt").Trim();

if (string.IsNullOrEmpty(input) || !input.All(char.IsDigit))
{
    Console.WriteLine("La entrada debe contener solo dígitos.");
    return;
}

int part1 = SolveCaptcha(input, step: 1);
int part2 = SolveCaptcha(input, step: input.Length / 2);

Console.WriteLine($"Parte 1: {part1}");
Console.WriteLine($"Parte 2: {part2}");

static int SolveCaptcha(string digits, int step)
{
    int length = digits.Length;
    int sum = 0;

    for (int i = 0; i < length; i++)
    {
        int nextIndex = (i + step) % length;
        if (digits[i] == digits[nextIndex])
        {
            sum += digits[i] - '0';
        }
    }

    return sum;
}
