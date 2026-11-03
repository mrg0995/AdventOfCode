using System;
using System.IO;
string line = string.Empty;
try
{
    //Pass the file path and file name to the StreamReader constructor
    StreamReader sr = new StreamReader("C:\\Users\\ProtoMkUser\\Desktop\\InputDi1.txt");

    //Read the first line of text
    line += sr.ReadLine();

    //close the file
    sr.Close();
}
catch (Exception e)
{
    Console.WriteLine("Exception: " + e.Message);
}

int planta = 0;
const char subirPlanta = '(';
const char bajarPlanta = ')';

for (int i = 0; i < line.Length; i++)
{
    if (line[i] == subirPlanta)
    {
        planta++;
    }

    if (line[i] == bajarPlanta)
    {
        planta--;
    }

    if (planta == -1)
    {
        Console.WriteLine($"En la posición {i + 1}, el elfo en la planta {planta}");
    }
}

Console.WriteLine($"Tras leer el fichero, la planta en la que nos encontramos es: {planta}");

