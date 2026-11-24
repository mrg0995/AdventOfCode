import re
from pathlib import Path

def solve(lines: list[str]):
    grid1 = [[0] * 1000 for _ in range(1000)]
    grid2 = [[0] * 1000 for _ in range(1000)]
    pattern = re.compile(r"(turn on|turn off|toggle) (\d+),(\d+) through (\d+),(\d+)")

    for line in lines:
        m = pattern.match(line.strip())
        if not m:
            continue
        cmd, x1, y1, x2, y2 = m.groups()
        x1, y1, x2, y2 = int(x1), int(y1), int(x2), int(y2)
        for x in range(x1, x2 + 1):
            for y in range(y1, y2 + 1):
                if cmd == "turn on":
                    grid1[x][y] = 1
                    grid2[x][y] += 1
                elif cmd == "turn off":
                    grid1[x][y] = 0
                    grid2[x][y] = max(0, grid2[x][y] - 1)
                else:
                    grid1[x][y] ^= 1
                    grid2[x][y] += 2

    return sum(sum(row) for row in grid1), sum(sum(row) for row in grid2)

if __name__ == "__main__":
    lines = Path("input_06.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 06 -> Parte 1: {p1} | Parte 2: {p2}")
