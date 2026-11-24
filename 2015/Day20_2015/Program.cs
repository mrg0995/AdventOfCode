using System;

long targetPresents = 36000000;

int maxHousesPart1 = (int)(targetPresents / 10);
int[] housesPart1 = new int[maxHousesPart1];

for (int elf = 1; elf < maxHousesPart1; elf++)
{
    int presents = elf * 10;
    for (int house = elf; house < maxHousesPart1; house += elf)
    {
        housesPart1[house] += presents;
    }
}

int housePart1Result = 0;
for (int i = 1; i < maxHousesPart1; i++)
{
    if (housesPart1[i] >= targetPresents)
    {
        housePart1Result = i;
        break;
    }
}

Console.WriteLine($"Parte 1 - Primera casa en recibir suficientes regalos: {housePart1Result}");

int maxHousesPart2 = (int)(targetPresents / 10);
int[] housesPart2 = new int[maxHousesPart2];

for (int elf = 1; elf < maxHousesPart2; elf++)
{
    int presents = elf * 11;
    int count = 0;

    for (int house = elf; house < maxHousesPart2 && count < 50; house += elf)
    {
        housesPart2[house] += presents;
        count++;
    }
}

int housePart2Result = 0;
for (int i = 1; i < maxHousesPart2; i++)
{
    if (housesPart2[i] >= targetPresents)
    {
        housePart2Result = i;
        break;
    }
}

Console.WriteLine($"Parte 2 - Primera casa en recibir suficientes regalos: {housePart2Result}");
