using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public class Item
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Armor { get; set; }

        public Item(string name, int cost, int damage, int armor)
        {
            Name = name;
            Cost = cost;
            Damage = damage;
            Armor = armor;
        }
    }

    static List<Item> weapons = new List<Item>
    {
        new Item("Dagger",      8,  4, 0),
        new Item("Shortsword", 10,  5, 0),
        new Item("Warhammer",  25,  6, 0),
        new Item("Longsword",  40,  7, 0),
        new Item("Greataxe",   74,  8, 0)
    };

    static List<Item> armors = new List<Item>
    {
        new Item("None",       0,  0, 0),
        new Item("Leather",   13,  0, 1),
        new Item("Chainmail", 31,  0, 2),
        new Item("Splintmail",53,  0, 3),
        new Item("Bandedmail",75,  0, 4),
        new Item("Platemail",102,  0, 5)
    };

    static List<Item> rings = new List<Item>
    {
        new Item("None1",      0,  0, 0),
        new Item("None2",      0,  0, 0),
        new Item("Damage +1", 25,  1, 0),
        new Item("Damage +2", 50,  2, 0),
        new Item("Damage +3",100,  3, 0),
        new Item("Defense +1", 20,  0, 1),
        new Item("Defense +2", 40,  0, 2),
        new Item("Defense +3", 80,  0, 3)
    };

    static bool Simulate(int playerHp, int playerDamage, int playerArmor, int bossHp, int bossDamage, int bossArmor)
    {
        int pDmgToBoss = Math.Max(1, playerDamage - bossArmor);
        int bDmgToPlayer = Math.Max(1, bossDamage - playerArmor);

        while (true)
        {
            bossHp -= pDmgToBoss;
            if (bossHp <= 0) return true;

            playerHp -= bDmgToPlayer;
            if (playerHp <= 0) return false;
        }
    }

    static void Main()
    {
        int bossInitialHp = 100;
        int bossDamage = 8;
        int bossArmor = 2;

        int minCostToWin = int.MaxValue;
        int maxCostToLose = 0;

        foreach (var weapon in weapons)
        {
            foreach (var armor in armors)
            {
                for (int i = 0; i < rings.Count; i++)
                {
                    for (int j = i + 1; j < rings.Count; j++)
                    {
                        var ring1 = rings[i];
                        var ring2 = rings[j];

                        int totalCost = weapon.Cost + armor.Cost + ring1.Cost + ring2.Cost;
                        int totalDamage = weapon.Damage + armor.Damage + ring1.Damage + ring2.Damage;
                        int totalArmor = weapon.Armor + armor.Armor + ring1.Armor + ring2.Armor;

                        bool playerWins = Simulate(
                            playerHp: 100, 
                            playerDamage: totalDamage, 
                            playerArmor: totalArmor, 
                            bossHp: bossInitialHp, 
                            bossDamage: bossDamage, 
                            bossArmor: bossArmor
                        );

                        if (playerWins)
                        {
                            if (totalCost < minCostToWin)
                            {
                                minCostToWin = totalCost;
                            }
                        }
                        else
                        {
                            if (totalCost > maxCostToLose)
                            {
                                maxCostToLose = totalCost;
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine($"Parte 1 - Mínimo oro para ganar: {minCostToWin}");
        Console.WriteLine($"Parte 2 - Máximo oro para perder: {maxCostToLose}");
    }
}