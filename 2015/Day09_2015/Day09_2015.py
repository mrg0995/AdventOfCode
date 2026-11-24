from itertools import permutations
from pathlib import Path

def solve(lines: list[str]):
    dist = {}
    cities = set()
    for line in lines:
        parts = line.strip().split()
        if len(parts) == 5:
            a, _, b, _, d = parts
            d = int(d)
            cities.update([a, b])
            dist[(a, b)] = dist[(b, a)] = d

    routes = [
        sum(dist[(route[i], route[i + 1])] for i in range(len(route) - 1))
        for route in permutations(cities)
    ]
    return min(routes), max(routes)

if __name__ == "__main__":
    lines = Path("input_09.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 09 -> Parte 1: {p1} | Parte 2: {p2}")
