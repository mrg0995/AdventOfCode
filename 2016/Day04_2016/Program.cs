using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

string[] lines = File.ReadAllLines(@"Your_Route");

int sumOfSectorIds = 0;
int northPoleSectorId = 0;

Regex regex = new Regex(@"^([a-z-]+)-(\d+)\[([a-z]+)\]$");

foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line)) continue;

    Match match = regex.Match(line);
    if (!match.Success) continue;

    string encryptedName = match.Groups[1].Value;
    int sectorId = int.Parse(match.Groups[2].Value);
    string givenChecksum = match.Groups[3].Value;

    var letterCounts = encryptedName
        .Where(c => c != '-')
        .GroupBy(c => c)
        .OrderByDescending(g => g.Count())
        .ThenBy(g => g.Key)
        .Select(g => g.Key)
        .Take(5);

    string calculatedChecksum = new string(letterCounts.ToArray());

    if (calculatedChecksum == givenChecksum)
    {
        sumOfSectorIds += sectorId;

        string decryptedName = DecryptName(encryptedName, sectorId);

        if (decryptedName.Contains("northpole"))
        {
            northPoleSectorId = sectorId;
        }
    }
}

Console.WriteLine($"Resultado Parte 1 (Suma de Sector IDs): {sumOfSectorIds}");
Console.WriteLine($"Resultado Parte 2 (Sector ID de North Pole): {northPoleSectorId}");

static string DecryptName(string encryptedName, int shift)
{
    StringBuilder sb = new StringBuilder();
    int effectiveShift = shift % 26;

    foreach (char c in encryptedName)
    {
        if (c == '-')
        {
            sb.Append(' ');
        }
        else
        {
            char decrypted = (char)('a' + (c - 'a' + effectiveShift) % 26);
            sb.Append(decrypted);
        }
    }

    return sb.ToString();
}