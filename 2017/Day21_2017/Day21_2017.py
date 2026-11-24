from pathlib import Path
import sys

INITIAL = [
    [".", "#", "."],
    [".", ".", "#"],
    ["#", "#", "#"],
]

def rotate_cw(grid: list[list[str]]) -> list[list[str]]:
    n = len(grid)
    return [[grid[n - 1 - c][r] for c in range(n)] for r in range(n)]

def flip_h(grid: list[list[str]]) -> list[list[str]]:
    return [row[::-1] for row in grid]

def serialize(grid: list[list[str]]) -> str:
    return "/".join("".join(row) for row in grid)

def parse_pattern(pat: str) -> list[list[str]]:
    return [list(row) for row in pat.strip().split("/")]

def load_rules(lines: list[str]) -> dict[str, list[list[str]]]:
    rules: dict[str, list[list[str]]] = {}
    for line in lines:
        line = line.strip()
        if not line:
            continue
        lhs, rhs = line.split("=>", 1)
        inp = parse_pattern(lhs)
        out = parse_pattern(rhs)
        curr = inp
        for _ in range(4):
            rules[serialize(curr)] = out
            rules[serialize(flip_h(curr))] = out
            curr = rotate_cw(curr)
    return rules

def step_grid(grid: list[list[str]], rules: dict[str, list[list[str]]]) -> list[list[str]]:
    size = len(grid)
    in_sz = 2 if size % 2 == 0 else 3
    out_sz = in_sz + 1
    blocks = size // in_sz
    new_sz = blocks * out_sz
    out_grid = [[""] * new_sz for _ in range(new_sz)]

    for br in range(blocks):
        for bc in range(blocks):
            sub = [
                [grid[br * in_sz + r][bc * in_sz + c] for c in range(in_sz)]
                for r in range(in_sz)
            ]
            expanded = rules[serialize(sub)]
            for r in range(out_sz):
                for c in range(out_sz):
                    out_grid[br * out_sz + r][bc * out_sz + c] = expanded[r][c]

    return out_grid

def count_lit_after(rules: dict[str, list[list[str]]], iterations: int) -> int:
    grid = [row[:] for row in INITIAL]
    for _ in range(iterations):
        grid = step_grid(grid, rules)
    return sum(1 for row in grid for ch in row if ch == "#")

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input21.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    rules = load_rules(lines)
    print(f"Día 21 - Parte 1: {count_lit_after(rules, 5)}")
    print(f"Día 21 - Parte 2: {count_lit_after(rules, 18)}")

if __name__ == "__main__":
    main()