from pathlib import Path

def solve(lines: list[str]):
    stats = {}
    for line in lines:
        if ":" in line:
            k, v = line.split(":", 1)
            stats[k.strip()] = int(v.strip())
    init_boss_hp = stats["Hit Points"]
    boss_dmg = stats["Damage"]

    spells = [
        (53, 4, 0, 0, 0, 0), 
        (73, 2, 2, 0, 0, 0),  
        (113, 0, 0, 6, 0, 0), 
        (173, 0, 0, 0, 6, 0),  
        (229, 0, 0, 0, 0, 5), 
    ]

    def simulate(hard_mode: bool):
        best = float("inf")

        def dfs(hp, mana, bhp, shield, poison, recharge, spent, player_turn):
            nonlocal best
            if spent >= best:
                return

            if hard_mode and player_turn:
                hp -= 1
                if hp <= 0:
                    return

            arm = 7 if shield > 0 else 0
            if shield > 0:
                shield -= 1
            if poison > 0:
                bhp -= 3
                poison -= 1
            if recharge > 0:
                mana += 101
                recharge -= 1

            if bhp <= 0:
                best = spent
                return

            if player_turn:
                for cost, dmg, heal, sh, ps, rc in spells:
                    if mana < cost:
                        continue
                    if sh > 0 and shield > 0:
                        continue
                    if ps > 0 and poison > 0:
                        continue
                    if rc > 0 and recharge > 0:
                        continue
                    dfs(
                        hp + heal,
                        mana - cost,
                        bhp - dmg,
                        sh if sh > 0 else shield,
                        ps if ps > 0 else poison,
                        rc if rc > 0 else recharge,
                        spent + cost,
                        False,
                    )
            else:
                dmg_taken = max(1, boss_dmg - arm)
                hp -= dmg_taken
                if hp > 0:
                    dfs(hp, mana, bhp, shield, poison, recharge, spent, True)

        dfs(50, 500, init_boss_hp, 0, 0, 0, 0, True)
        return best

    return simulate(False), simulate(True)

if __name__ == "__main__":
    lines = Path("input_22.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 22 -> Parte 1: {p1} | Parte 2: {p2}")
