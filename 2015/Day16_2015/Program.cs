using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static readonly Dictionary<string, int> TickerTape = new Dictionary<string, int>
    {
        { "children", 3 }, { "cats", 7 }, { "samoyeds", 2 },
        { "pomeranians", 3 }, { "akitas", 0 }, { "vizslas", 0 },
        { "goldfish", 5 }, { "trees", 3 }, { "cars", 2 }, { "perfumes", 1 }
    };

    static void Main()
    {
        string rutaArchivo = @"Your_Route";

        if (!File.Exists(rutaArchivo))
        {
            Console.WriteLine($"No se encontró el archivo en: {rutaArchivo}");
            return;
        }

        Console.WriteLine($"Parte 1 - Tía correcta: {ResolverParte1(rutaArchivo)}");
        Console.WriteLine($"Parte 2 - Tía correcta: {ResolverParte2(rutaArchivo)}");
    }

    static int ResolverParte1(string ruta)
    {
        foreach (string linea in File.ReadLines(ruta))
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            int numeroSue = ObtenerNumeroSue(linea);
            var propiedades = ExtraerPropiedades(linea);
            bool esLaTiaCorrecta = true;

            foreach (var prop in propiedades)
            {
                if (TickerTape.ContainsKey(prop.Key) && TickerTape[prop.Key] != prop.Value)
                {
                    esLaTiaCorrecta = false;
                    break;
                }
            }

            if (!esLaTiaCorrecta) continue;

            return numeroSue;
        }

        return -1;
    }

    static int ResolverParte2(string ruta)
    {
        foreach (string linea in File.ReadLines(ruta))
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            int numeroSue = ObtenerNumeroSue(linea);
            var propiedades = ExtraerPropiedades(linea);
            bool esLaTiaCorrecta = true;

            foreach (var prop in propiedades)
            {
                if (!TickerTape.ContainsKey(prop.Key)) continue;

                int valorObjetivo = TickerTape[prop.Key];

                bool cumple = prop.Key switch
                {
                    "cats" or "trees" => prop.Value > valorObjetivo,
                    "pomeranians" or "goldfish" => prop.Value < valorObjetivo,
                    _ => prop.Value == valorObjetivo
                };

                if (!cumple)
                {
                    esLaTiaCorrecta = false;
                    break;
                }
            }

            if (!esLaTiaCorrecta) continue;

            return numeroSue;
        }

        return -1;
    }

    static int ObtenerNumeroSue(string linea)
    {
        Match match = Regex.Match(linea, @"Sue (\d+):");
        return int.Parse(match.Groups[1].Value);
    }

    static Dictionary<string, int> ExtraerPropiedades(string linea)
    {
        var dict = new Dictionary<string, int>();
        MatchCollection matches = Regex.Matches(linea, @"([a-z]+):\s*(\d+)");

        foreach (Match m in matches)
        {
            dict[m.Groups[1].Value] = int.Parse(m.Groups[2].Value);
        }

        return dict;
    }
}