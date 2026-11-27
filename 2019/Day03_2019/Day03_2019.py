from pathlib import Path

DIRS = {
    "R": (1, 0),
    "L": (-1, 0),
    "U": (0, 1),
    "D": (0, -1),
}

def trace_wire(path_str: str) -> dict[tuple[int, int], int]:
    x = y = steps = 0
    visited: dict[tuple[int, int], int] = {}
    for token in path_str.strip().split(","):
        if not token:
            continue
        dx, dy = DIRS[token[0]]
        dist = int(token[1:])
        for _ in range(dist):
            x += dx
            y += dy
            steps += 1
            pos = (x, y)
            if pos not in visited:
                visited[pos] = steps
    return visited

def solve(lines: list[str]) -> tuple[int, int]:
    w1 = trace_wire(lines[0])
    w2 = trace_wire(lines[1])
    intersections = w1.keys() & w2.keys()

    p1 = min(abs(x) + abs(y) for x, y in intersections)
    p2 = min(w1[pos] + w2[pos] for pos in intersections)
    return p1, p2

if __name__ == "__main__":
    lines = [l for l in Path("Day03_2019.txt").read_text(encoding="utf-8").splitlines() if l.strip()]
    p1, p2 = solve(lines)
    print(f"Day 3 - Part 1: {p1}, Part 2: {p2}")