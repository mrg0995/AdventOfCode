using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

string rutaArchivo = @"Your_Route";
string contenido = File.ReadAllText(rutaArchivo);

string patron = @"-?\d+(?:\.\d+)?";
MatchCollection coincidencias = Regex.Matches(contenido, patron);

double sumaParte1 = 0;
foreach (Match match in coincidencias)
{
    if (double.TryParse(match.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double numero))
    {
        sumaParte1 += numero;
    }
}

Console.WriteLine($"Suma total de todos los números (con negativos): {sumaParte1}\n");

using (JsonDocument doc = JsonDocument.Parse(contenido))
{
    int sumaParte2 = SumarJsonSinRojos(doc.RootElement);
    Console.WriteLine($"=== PARTE 2 ===");
    Console.WriteLine($"Suma total ignorando objetos 'red': {sumaParte2}");
}

static int SumarJsonSinRojos(JsonElement element)
{
    switch (element.ValueKind)
    {
        case JsonValueKind.Number:
            return element.GetInt32();

        case JsonValueKind.Array:
            int sumaArray = 0;
            foreach (var item in element.EnumerateArray())
            {
                sumaArray += SumarJsonSinRojos(item);
            }
            return sumaArray;

        case JsonValueKind.Object:
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String && prop.Value.GetString() == "red")
                {
                    return 0;
                }
            }

            int sumaObjeto = 0;
            foreach (var prop in element.EnumerateObject())
            {
                sumaObjeto += SumarJsonSinRojos(prop.Value);
            }
            return sumaObjeto;

        default:
            return 0;
    }
}