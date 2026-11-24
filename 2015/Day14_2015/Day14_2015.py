import re
from pathlib import Path

def solve(lines: list[str], seconds: int = 2503):
    deer = []
    pat = re.compile(r"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.")
    for line in lines:
        m = pat.search(line)
        if m:
            name, spd, fly, rest = m.group(1), int(m.group(2)), int(m.group(3)), int(m.group(4))
            deer.append((name, spd, fly, rest))

    def dist_at(spd, fly, rest, t):
        cycle = fly + rest
        full, rem = divmod(t, cycle)
        return spd * (full * fly + min(rem, fly))

    p1 = max(dist_at(spd, fly, rest, seconds) for _, spd, fly, rest in deer)

    points = {name: 0 for name, *_ in deer}
    for t in range(1, seconds + 1):
        positions = {name: dist_at(spd, fly, rest, t) for name, spd, fly, rest in deer}
        lead = max(positions.values())
        for name, pos in positions.items():
            if pos == lead:
                points[name] += 1

    return p1, max(points.values())

if __name__ == "__main__":
    lines = Path("input_14.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 14 -> Parte 1: {p1} | Parte 2: {p2}")
