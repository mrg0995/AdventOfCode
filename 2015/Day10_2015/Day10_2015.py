from itertools import groupby
from pathlib import Path

def solve(seed: str):
    def look_and_say(s: str, steps: int):
        for _ in range(steps):
            s = "".join(f"{len(list(g))}{k}" for k, g in groupby(s))
        return len(s)

    s = seed.strip()
    return look_and_say(s, 40), look_and_say(s, 50)

if __name__ == "__main__":
    raw = Path("input_10.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 10 -> Parte 1: {p1} | Parte 2: {p2}")
