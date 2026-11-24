from pathlib import Path

def solve(data: str):
    floor = 0
    first_basement = None
    for idx, ch in enumerate(data.strip(), start=1):
        floor += 1 if ch == "(" else -1
        if floor == -1 and first_basement is None:
            first_basement = idx
    return floor, first_basement

if __name__ == "__main__":
    raw = Path("input_01.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 01 -> Parte 1: {p1} | Parte 2: {p2}")
