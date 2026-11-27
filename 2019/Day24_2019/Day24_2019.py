from collections import defaultdict
from pathlib import Path

def biodiversity(grid: list[list[bool]]) -> int:
    score = 0
    for r in range(5):
        for c in range(5):
            if grid[r][c]:
                score += 1 << (r * 5 + c)
    return score

def step_p1(grid: list[list[bool]]) -> list[list[bool]]:
    nxt = [[False] * 5 for _ in range(5)]
    dirs = ((-1, 0), (1, 0), (0, -1), (0, 1))
    for r in range(5):
        for c in range(5):
            adj = 0
            for dr, dc in dirs:
                nr, nc = r + dr, c + dc
                if 0 <= nr < 5 and 0 <= nc < 5 and grid[nr][nc]:
                    adj += 1
            if grid[r][c]:
                nxt[r][c] = adj == 1
            else:
                nxt[r][c] = adj in (1, 2)
    return nxt

def adjacent_bugs_p2(levels: dict[int, list[list[bool]]], lvl: int, r: int, c: int) -> int:
    count = 0
    for dr, dc in ((-1, 0), (1, 0), (0, -1), (0, 1)):
        nr, nc = r + dr, c + dc
        if nr == 2 and nc == 2:
            inner = levels.get(lvl + 1)
            if inner is not None:
                if r == 1 and c == 2:
                    count += sum(1 for x in range(5) if inner[0][x])
                elif r == 3 and c == 2:
                    count += sum(1 for x in range(5) if inner[4][x])
                elif r == 2 and c == 1: 
                    count += sum(1 for y in range(5) if inner[y][0])
                elif r == 2 and c == 3:
                    count += sum(1 for y in range(5) if inner[y][4])
        elif nr < 0:
            outer = levels.get(lvl - 1)
            if outer is not None and outer[1][2]:
                count += 1
        elif nr >= 5:
            outer = levels.get(lvl - 1)
            if outer is not None and outer[3][2]:
                count += 1
        elif nc < 0:
            outer = levels.get(lvl - 1)
            if outer is not None and outer[2][1]:
                count += 1
        elif nc >= 5:
            outer = levels.get(lvl - 1)
            if outer is not None and outer[2][3]:
                count += 1
        else:
            g = levels.get(lvl)
            if g is not None and g[nr][nc]:
                count += 1
    return count

def solve(lines: list[str]) -> tuple[int, int]:
    rows = [l.strip() for l in lines if l.strip()]
    init_grid = [[rows[r][c] == "#" for c in range(5)] for r in range(5)]

    cur = [row[:] for row in init_grid]
    seen: set[int] = {biodiversity(cur)}
    p1 = 0
    while True:
        cur = step_p1(cur)
        b = biodiversity(cur)
        if b in seen:
            p1 = b
            break
        seen.add(b)

    levels: dict[int, list[list[bool]]] = {0: [row[:] for row in init_grid]}
    for _ in range(200):
        min_l = min(levels.keys()) - 1
        max_l = max(levels.keys()) + 1
        next_levels: dict[int, list[list[bool]]] = {}
        for lvl in range(min_l, max_l + 1):
            ng = [[False] * 5 for _ in range(5)]
            for r in range(5):
                for c in range(5):
                    if r == 2 and c == 2:
                        continue
                    adj = adjacent_bugs_p2(levels, lvl, r, c)
                    has_bug = levels.get(lvl, [[False] * 5 for _ in range(5)])[r][c]
                    if has_bug:
                        ng[r][c] = adj == 1
                    else:
                        ng[r][c] = adj in (1, 2)
            next_levels[lvl] = ng
        levels = next_levels

    p2 = sum(
        1
        for g in levels.values()
        for r in range(5)
        for c in range(5)
        if not (r == 2 and c == 2) and g[r][c]
    )
    return p1, p2

if __name__ == "__main__":
    lines = Path("Day24_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 24 - Part 1: {p1}, Part 2: {p2}")