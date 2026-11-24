from collections import deque
from itertools import combinations, permutations
from pathlib import Path


def solve(lines: list[str]):
    grid = [line.rstrip("\n") for line in lines if line.strip()]
    h, w = len(grid), len(grid[0])
    pois = {}

    for r in range(h):
        for c in range(w):
            ch = grid[r][c]
            if ch.isdigit():
                pois[int(ch)] = (r, c)

    def shortest_path(start, goal):
        if start == goal:
            return 0
        q = deque([(start[0], start[1], 0)])
        seen = {start}
        while q:
            r, c, d = q.popleft()
            for dr, dc in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                nr, nc = r + dr, c + dc
                if 0 <= nr < h and 0 <= nc < w and grid[nr][nc] != "#" and (nr, nc) not in seen:
                    if (nr, nc) == goal:
                        return d + 1
                    seen.add((nr, nc))
                    q.append((nr, nc, d + 1))
        return float("inf")

    keys = sorted(pois.keys())
    dist = {}
    for a, b in combinations(keys, 2):
        d = shortest_path(pois[a], pois[b])
        dist[(a, b)] = dist[(b, a)] = d

    others = [k for k in keys if k != 0]
    best_p1 = float("inf")
    best_p2 = float("inf")

    for order in permutations(others):
        cost = dist[(0, order[0])] + sum(
            dist[(order[i], order[i + 1])] for i in range(len(order) - 1)
        )
        if cost < best_p1:
            best_p1 = cost
        cost_back = cost + dist[(order[-1], 0)]
        if cost_back < best_p2:
            best_p2 = cost_back

    return best_p1, best_p2


if __name__ == "__main__":
    lines = Path("input_24.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 24 -> Parte 1: {p1} | Parte 2: {p2}")
