from pathlib import Path

def solve(target_str: str):
    target = int(target_str.strip())
    limit1 = target // 10
    houses1 = [0] * (limit1 + 1)
    for elf in range(1, limit1 + 1):
        for h in range(elf, limit1 + 1, elf):
            houses1[h] += elf * 10
    p1 = next(i for i, v in enumerate(houses1) if v >= target)

    limit2 = target // 11
    houses2 = [0] * (limit2 + 1)
    for elf in range(1, limit2 + 1):
        visited = 0
        for h in range(elf, limit2 + 1, elf):
            houses2[h] += elf * 11
            visited += 1
            if visited == 50:
                break
    p2 = next(i for i, v in enumerate(houses2) if v >= target)

    return p1, p2

if __name__ == "__main__":
    raw = Path("input_20.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 20 -> Parte 1: {p1} | Parte 2: {p2}")
