from pathlib import Path
import sys

DIRS = [(-1, 0), (0, 1), (1, 0), (0, -1)]

def parse_center_grid(lines: list[str]) -> set[tuple[int, int]]:
    rows = len(lines)
    cols = len(lines[0])
    cr, cc = rows // 2, cols // 2
    infected: set[tuple[int, int]] = set()
    for r in range(rows):
        for c in range(cols):
            if lines[r][c] == "#":
                infected.add((r - cr, c - cc))
    return infected

def solve_part1(lines: list[str], bursts: int = 10_000) -> int:
    infected = parse_center_grid(lines)
    r = c = 0
    d = 0
    caused = 0

    for _ in range(bursts):
        pos = (r, c)
        if pos in infected:
            d = (d + 1) % 4
            infected.remove(pos)
        else:
            d = (d + 3) % 4
            infected.add(pos)
            caused += 1
        r += DIRS[d][0]
        c += DIRS[d][1]

    return caused

def solve_part2(lines: list[str], bursts: int = 10_000_000) -> int:
    states = {pos: 2 for pos in parse_center_grid(lines)}
    r = c = 0
    d = 0
    caused = 0

    for _ in range(bursts):
        pos = (r, c)
        st = states.get(pos, 0)
        if st == 0:
            d = (d + 3) % 4
            states[pos] = 1
        elif st == 1:
            states[pos] = 2
            caused += 1
        elif st == 2:
            d = (d + 1) % 4
            states[pos] = 3
        else:
            d = (d + 2) % 4
            states.pop(pos, None)
        r += DIRS[d][0]
        c += DIRS[d][1]

    return caused

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input22.txt")
    lines = (
        [l.strip() for l in path.read_text(encoding="utf-8").splitlines() if l.strip()]
        if path.exists()
        else ["..#", "#..", "..."]
    )
    print(f"Día 22 - Parte 1: {solve_part1(lines)}")
    print(f"Día 22 - Parte 2: {solve_part2(lines)}")

if __name__ == "__main__":
    main()