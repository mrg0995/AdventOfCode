from itertools import combinations, product
from pathlib import Path

def solve(lines: list[str]):
    boss_stats = {}
    for line in lines:
        if ":" in line:
            k, v = line.split(":", 1)
            boss_stats[k.strip()] = int(v.strip())
    boss_hp = boss_stats["Hit Points"]
    boss_dmg = boss_stats["Damage"]
    boss_arm = boss_stats["Armor"]

    weapons = [(8, 4, 0), (10, 5, 0), (25, 6, 0), (40, 7, 0), (74, 8, 0)]
    armors = [(0, 0, 0), (13, 0, 1), (31, 0, 2), (53, 0, 3), (75, 0, 4), (102, 0, 5)]
    rings = [
        (0, 0, 0), (0, 0, 0),
        (25, 1, 0), (50, 2, 0), (100, 3, 0),
        (20, 0, 1), (40, 0, 2), (80, 0, 3),
    ]

    def player_wins(dmg, arm):
        p_hit = max(1, dmg - boss_arm)
        b_hit = max(1, boss_dmg - arm)
        turns_to_kill_boss = (boss_hp + p_hit - 1) // p_hit
        turns_to_kill_me = (100 + b_hit - 1) // b_hit
        return turns_to_kill_boss <= turns_to_kill_me

    min_win_cost = float("inf")
    max_lose_cost = 0

    for w, a, r_pair in product(weapons, armors, combinations(rings, 2)):
        cost = w[0] + a[0] + r_pair[0][0] + r_pair[1][0]
        dmg = w[1] + a[1] + r_pair[0][1] + r_pair[1][1]
        arm = w[2] + a[2] + r_pair[0][2] + r_pair[1][2]
        if player_wins(dmg, arm):
            if cost < min_win_cost:
                min_win_cost = cost
        else:
            if cost > max_lose_cost:
                max_lose_cost = cost

    return min_win_cost, max_lose_cost

if __name__ == "__main__":
    lines = Path("input_21.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 21 -> Parte 1: {p1} | Parte 2: {p2}")
