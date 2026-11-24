from pathlib import Path
import sys

def parse_components(lines: list[str]) -> list[tuple[int, int]]:
    comps: list[tuple[int, int]] = []
    for line in lines:
        line = line.strip()
        if not line:
            continue
        a_str, b_str = line.split("/", 1)
        comps.append((int(a_str), int(b_str)))
    return comps

def search_bridges(components: list[tuple[int, int]]) -> tuple[int, int]:
    n = len(components)
    used = [False] * n
    best_strength = 0
    best_len = 0
    best_len_strength = 0

    def backtrack(port: int, length: int, strength: int) -> None:
        nonlocal best_strength, best_len, best_len_strength

        if strength > best_strength:
            best_strength = strength

        if length > best_len or (length == best_len and strength > best_len_strength):
            best_len = length
            best_len_strength = strength

        for i in range(n):
            if used[i]:
                continue
            a, b = components[i]
            if a == port:
                nxt = b
            elif b == port:
                nxt = a
            else:
                continue

            used[i] = True
            backtrack(nxt, length + 1, strength + a + b)
            used[i] = False

    backtrack(0, 0, 0)
    return best_strength, best_len_strength

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input24.txt")
    if path.exists():
        lines = path.read_text(encoding="utf-8").splitlines()
    else:
        lines = ["0/2", "2/2", "2/3", "3/4", "3/5", "0/1", "10/1", "9/10"]
    comps = parse_components(lines)
    part1, part2 = search_bridges(comps)
    print(f"Día 24 - Parte 1: {part1}")
    print(f"Día 24 - Parte 2: {part2}")

if __name__ == "__main__":
    main()