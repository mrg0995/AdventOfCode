from pathlib import Path


def solve(data: str):
    dirs = [(0, 1), (1, 0), (0, -1), (-1, 0)]  # N, E, S, W
    d = 0
    x = y = 0
    visited = {(0, 0)}
    first_revisit = None

    for token in data.strip().split(", "):
        if not token:
            continue
        turn = token[0]
        steps = int(token[1:])
        d = (d + (1 if turn == "R" else -1)) % 4
        dx, dy = dirs[d]
        for _ in range(steps):
            x += dx
            y += dy
            if first_revisit is None and (x, y) in visited:
                first_revisit = abs(x) + abs(y)
            visited.add((x, y))

    p1 = abs(x) + abs(y)
    return p1, first_revisit


if __name__ == "__main__":
    raw = Path("input_01.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 01 -> Parte 1: {p1} | Parte 2: {p2}")
