from pathlib import Path


def is_valid(a: int, b: int, c: int) -> bool:
    return a + b > c and a + c > b and b + c > a


def solve(lines: list[str]):
    rows = []
    for line in lines:
        parts = [int(x) for x in line.strip().split()]
        if len(parts) == 3:
            rows.append(parts)

    p1 = sum(1 for a, b, c in rows if is_valid(a, b, c))

    p2 = 0
    for r in range(0, len(rows) - 2, 3):
        for col in range(3):
            if is_valid(rows[r][col], rows[r + 1][col], rows[r + 2][col]):
                p2 += 1

    return p1, p2


if __name__ == "__main__":
    lines = Path("input_03.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 03 -> Parte 1: {p1} | Parte 2: {p2}")
