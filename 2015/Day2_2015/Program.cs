using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

string line = string.Empty;
List<string> presents = new List<string>();

try
{
    StreamReader sr = new StreamReader("Your_Route");

    line = sr.ReadLine();

    while (line != null && !string.IsNullOrWhiteSpace(line))
    {
        presents.Add(line);
        line = sr.ReadLine();
    }

        sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

int areaTotal = 0;
int ribbonTotal = 0;

for (int i = 0; i < presents.Count; i++)
{
    int l = 0;
    int w = 0;
    int h = 0;


    List<int> numeros = Regex.Matches(presents[i], @"\d+")
                   .Select(m => int.Parse(m.Value))
                   .ToList();

    l = numeros[0];
    w = numeros[1];
    h = numeros[2];

    int area1 = l * w;
    int area2 = w * h;
    int area3 = h * l;

    int areaMenor = 0;

    if (area1 < area2)
    {
        areaMenor = area1;
    }
    else
    {
        areaMenor = area2;
    }

    if (areaMenor > area3)
    {
        areaMenor = area3;
    }

    int areaTotalPresent = 2 * area1 + 2 * area2 + 2 * area3 + areaMenor;
    areaTotal += areaTotalPresent;

    int ladoMayor = 0;

    if (l < w)
    {
        ladoMayor = w;
    }
    else
    {
        ladoMayor = l;
    }

    if (ladoMayor < h)
    {
        ladoMayor = h;
    }

    int ribbon = 2 * (l + w + h - ladoMayor);
    int bow = l * w * h;
    ribbonTotal += ribbon + bow;
}

Console.WriteLine($"El área total es {areaTotal}");
Console.WriteLine($"El lazo total es {ribbonTotal}");
