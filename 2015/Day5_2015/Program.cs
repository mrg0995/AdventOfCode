using System;
using System.Linq;
using System.Text.RegularExpressions;

string line = string.Empty;
List<string> words = new List<string>();

try
{
    StreamReader sr = new StreamReader("C:\\Users\\ProtoMkUser\\Desktop\\Day5_2015.txt");

    line = sr.ReadLine();

    while (line != null && !string.IsNullOrWhiteSpace(line))
    {
        words.Add(line);
        line = sr.ReadLine();
    }

    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

/* 
static bool EsCadenaValida(string s)
{
    bool tieneTresVocales = s.Count(c => "aeiou".Contains(c)) >= 3;

    bool tieneDobleLetra = Regex.IsMatch(s, @"([a-z])\1");

    bool sinSubcadenasProhibidas = !s.Contains("ab") &&
                                   !s.Contains("cd") &&
                                   !s.Contains("pq") &&
                                   !s.Contains("xy");

    return tieneTresVocales && tieneDobleLetra && sinSubcadenasProhibidas;
}
*/
static bool EsCadenaValida(string s)
{
    bool parRepetidoNoSolapado = Regex.IsMatch(s, @"([a-z]{2}).*\1");

    bool letraRepetidaConUnaEntreMedias = Regex.IsMatch(s, @"([a-z]).\1");

    return parRepetidoNoSolapado && letraRepetidaConUnaEntreMedias;
}

int niceCount = 0;

foreach (string word in words)
{
    if (EsCadenaValida(word))
    {
        niceCount++;
    }
   
}

Console.WriteLine($"El número de strings que son nice son {niceCount}");
    