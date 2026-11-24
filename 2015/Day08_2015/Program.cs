using System;
using System.IO;

string filePath = "Your_Route";

int totalCodeLength = 0;
int totalMemoryLength = 0;
int totalEncodedLength = 0;

foreach (var rawLine in File.ReadLines(filePath))
{
    string line = rawLine.Trim();
    if (string.IsNullOrEmpty(line)) continue;

    totalCodeLength += line.Length;

    int memLen = 0;
    for (int i = 1; i < line.Length - 1; i++)
    {
        if (line[i] == '\\')
        {
            if (line[i + 1] == '\\' || line[i + 1] == '\"')
            {
                i++;
            }
            else if (line[i + 1] == 'x')
            {
                i += 3;
            }
        }
        memLen++;
    }
    totalMemoryLength += memLen;

    int specialCharsCount = 0;
    foreach (char c in line)
    {
        if (c == '\"' || c == '\\')
        {
            specialCharsCount++;
        }
    }
    int encodedLen = 2 + line.Length + specialCharsCount;
    totalEncodedLength += encodedLen;
}

Console.WriteLine($"Code Length - Memory Length: {totalCodeLength - totalMemoryLength}");
Console.WriteLine($"Encoded Length - Code Length: {totalEncodedLength - totalCodeLength}");