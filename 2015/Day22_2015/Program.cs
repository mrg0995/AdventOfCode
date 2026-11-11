using System;
using System.Collections.Generic;

class Program
{
    public class Spell
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Heal { get; set; }
        public int ArmorBonus { get; set; }
        public int ManaBonus { get; set; }
        public int Duration { get; set; }
    }

    public class State : IComparable<State>
    {
        public int PlayerHp { get; set; }
        public int PlayerMana { get; set; }
        public int BossHp { get; set; }
        public int BossDamage { get; set; }
        public int SpentMana { get; set; }

        public int ShieldTimer { get; set; }
        public int PoisonTimer { get; set; }
        public int RechargeTimer { get; set; }

        public bool IsPlayerTurn { get; set; }

        public int CompareTo(State other)
        {
            return SpentMana.CompareTo(other.SpentMana);
        }
    }

    static List<Spell> spells = new List<Spell>
    {
        new Spell { Name = "Magic Missile", Cost = 53, Damage = 4, Heal = 0, ArmorBonus = 0, ManaBonus = 0, Duration = 0 },
        new Spell { Name = "Drain", Cost = 73, Damage = 2, Heal = 2, ArmorBonus = 0, ManaBonus = 0, Duration = 0 },
        new Spell { Name = "Shield", Cost = 113, Damage = 0, Heal = 0, ArmorBonus = 7, ManaBonus = 0, Duration = 6 },
        new Spell { Name = "Poison", Cost = 173, Damage = 3, Heal = 0, ArmorBonus = 0, ManaBonus = 0, Duration = 6 },
        new Spell { Name = "Recharge", Cost = 229, Damage = 0, Heal = 0, ArmorBonus = 0, ManaBonus = 101, Duration = 5 }
    };

    static int Solve(int bossInitialHp, int bossDamage, bool hardMode)
    {
        var pq = new PriorityQueue<State, int>();

        var initialState = new State
        {
            PlayerHp = 50,
            PlayerMana = 500,
            BossHp = bossInitialHp,
            BossDamage = bossDamage,
            SpentMana = 0,
            ShieldTimer = 0,
            PoisonTimer = 0,
            RechargeTimer = 0,
            IsPlayerTurn = true
        };

        pq.Enqueue(initialState, 0);

        while (pq.Count > 0)
        {
            var state = pq.Dequeue();

            if (state.BossHp <= 0) return state.SpentMana;

            if (state.IsPlayerTurn && hardMode)
            {
                state.PlayerHp--;
                if (state.PlayerHp <= 0) continue;
            }

            int currentArmor = 0;
            if (state.ShieldTimer > 0)
            {
                currentArmor = 7;
                state.ShieldTimer--;
            }
            if (state.PoisonTimer > 0)
            {
                state.BossHp -= 3;
                state.PoisonTimer--;
            }
            if (state.RechargeTimer > 0)
            {
                state.PlayerMana += 101;
                state.RechargeTimer--;
            }

            if (state.BossHp <= 0) return state.SpentMana;

            if (state.IsPlayerTurn)
            {
                foreach (var spell in spells)
                {
                    if (state.PlayerMana < spell.Cost) continue;
                    if (spell.Name == "Shield" && state.ShieldTimer > 0) continue;
                    if (spell.Name == "Poison" && state.PoisonTimer > 0) continue;
                    if (spell.Name == "Recharge" && state.RechargeTimer > 0) continue;

                    var nextState = new State
                    {
                        PlayerHp = state.PlayerHp,
                        PlayerMana = state.PlayerMana - spell.Cost,
                        BossHp = state.BossHp,
                        BossDamage = state.BossDamage,
                        SpentMana = state.SpentMana + spell.Cost,
                        ShieldTimer = state.ShieldTimer,
                        PoisonTimer = state.PoisonTimer,
                        RechargeTimer = state.RechargeTimer,
                        IsPlayerTurn = false
                    };

                    if (spell.Duration == 0)
                    {
                        nextState.BossHp -= spell.Damage;
                        nextState.PlayerHp += spell.Heal;
                    }
                    else
                    {
                        if (spell.Name == "Shield") nextState.ShieldTimer = spell.Duration;
                        if (spell.Name == "Poison") nextState.PoisonTimer = spell.Duration;
                        if (spell.Name == "Recharge") nextState.RechargeTimer = spell.Duration;
                    }

                    pq.Enqueue(nextState, nextState.SpentMana);
                }
            }
            else
            {
                if (state.BossHp <= 0) return state.SpentMana;

                int damageDealt = Math.Max(1, state.BossDamage - currentArmor);
                state.PlayerHp -= damageDealt;

                if (state.PlayerHp > 0)
                {
                    state.IsPlayerTurn = true;
                    pq.Enqueue(state, state.SpentMana);
                }
            }
        }

        return -1;
    }

    static void Main()
    {
        int bossHp = 71;
        int bossDamage = 10;

        int part1 = Solve(bossHp, bossDamage, hardMode: false);
        int part2 = Solve(bossHp, bossDamage, hardMode: true);

        Console.WriteLine($"Parte 1 - Menor cantidad de maná para ganar: {part1}");
        Console.WriteLine($"Parte 2 (Hard Mode) - Menor cantidad de maná para ganar: {part2}");
    }
}
