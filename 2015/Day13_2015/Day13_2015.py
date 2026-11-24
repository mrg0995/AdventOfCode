from itertools import permutations
from pathlib import Path

def solve(lines: list[str]):
    hap = {}
    guests = set()
    for line in lines:
        s = line.strip().rstrip(".")
        if not s:
            continue
        parts = s.split()
        a, _, sign, amt, _, _, _, _, _, _, b = parts
        val = int(amt) * (1 if sign == "gain" else -1)
        guests.update([a, b])
        hap[(a, b)] = val

    def best_happiness(people):
        people = list(people)
        first = people[0]
        others = people[1:]
        best = float("-inf")
        for perm in permutations(others):
            seating = [first] + list(perm)
            n = len(seating)
            total = 0
            for i in range(n):
                p = seating[i]
                left = seating[(i - 1) % n]
                right = seating[(i + 1) % n]
                total += hap.get((p, left), 0) + hap.get((p, right), 0)
            if total > best:
                best = total
        return best

    p1 = best_happiness(guests)
    p2 = best_happiness(guests | {"Me"})
    return p1, p2

if __name__ == "__main__":
    lines = Path("input_13.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 13 -> Parte 1: {p1} | Parte 2: {p2}")
