from pathlib import Path
import sys

def follow_tubes(raw_lines: list[str]) -> tuple[str, int]:
    rows = len(raw_lines)
    cols = max((len(line) for line in raw_lines), default=0)
    grid = [[line[c] if c < len(line) else " " for c in range(cols)] for line in raw_lines]

    start_col = grid[0].index("|")
    r, c = 0, start_col
    dr, dc = 1, 0
    letters: list[str] = []
    steps = 0

    while 0 <= r < rows and 0 <= c < cols and grid[r][c] != " ":
        steps += 1
        ch = grid[r][c]
        if ch.isalpha():
            letters.append(ch)
        elif ch == "+":
            for ndr, ndc in ((-dc, dr), (dc, -dr)):
                nr, nc = r + ndr, c + ndc
                if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] != " ":
                    dr, dc = ndr, ndc
                    break
        r += dr
        c += dc

    return "".join(letters), steps

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input19.txt")
    if path.exists():
        lines = path.read_text(encoding="utf-8").splitlines()
    else:
        lines = [
            "     |          ",
            "     |  +--+    ",
            "     A  |  C    ",
            " F---|----E|--+ ",
            "     |  |  |  D ",
            "     +B-+  +--+ ",
        ]
    part1, part2 = follow_tubes(lines)
    print(f"Día 19 - Parte 1: {part1}")
    print(f"Día 19 - Parte 2: {part2}")

if __name__ == "__main__":
    main()