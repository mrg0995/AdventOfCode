from collections import deque
from pathlib import Path


def solve(fav_str: str):
    fav = int(fav_str.strip())

    def is_open(x: int, y: int) -> bool:
        if x < 0 or y < 0:
            return False
        n = x * x + 3 * x + 2 * x * y + y + y * y + fav
        return bin(n).count("1") % 2 == 0

    q = deque([(1, 1, 0)])
    visited = {(1, 1)}
    p1 = None
    p2 = 0

    while q:
        x, y, dist = q.popleft()
        if dist <= 50:
            p2 += 1
        if (x, y) == (31, 39) and p1 is None:
            p1 = dist

        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
            nx, ny = x + dx, y + dy
            if (nx, ny) not in visited and is_open(nx, ny):
                visited.add((nx, ny))
                q.append((nx, ny, dist + 1))

    return p1, p2


if __name__ == "__main__":
    raw = Path("input_13.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 13 -> Parte 1: {p1} | Parte 2: {p2}")
