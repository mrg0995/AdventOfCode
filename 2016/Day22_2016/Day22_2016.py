from collections import deque
import heapq
from pathlib import Path
import re


def solve(lines: list[str]):
    grid = {}
    max_x = max_y = 0
    target = None

    for line in lines:
        m = re.match(r"^/dev/grid/node-x(\d+)-y(\d+)\s+(\d+)T\s+(\d+)T\s+(\d+)T\s+(\d+)%$", line.strip())
        if m:
            x, y, size, used, avail, _ = map(int, m.groups())
            grid[(x, y)] = (size, used, avail)
            max_x = max(max_x, x)
            max_y = max(max_y, y)
            if y == 0 and x > (target[0] if target else -1):
                target = (x, y)

    # Parte 1: pares viables (A != B, A usado > 0, A usado <= B disponible)
    viable = 0
    nodes = list(grid.items())
    for idx_a, (pos_a, (_, used_a, _)) in enumerate(nodes):
        if used_a == 0:
            continue
        for idx_b, (pos_b, (_, _, avail_b)) in enumerate(nodes):
            if idx_a != idx_b and used_a <= avail_b:
                viable += 1

    # Parte 2: camino más corto moviendo hueco '_' hasta llevar carga de (max_x, 0) a (0, 0)
    empty_start = next(pos for pos, (_, used, _) in grid.items() if used == 0)
    walls = {pos for pos, (_, used, _) in grid.items() if used > 100}

    def bfs_empty(start_e, goal_e, blocked):
        if start_e == goal_e:
            return 0
        q = deque([(start_e[0], start_e[1], 0)])
        seen = {start_e}
        while q:
            x, y, d = q.popleft()
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, ny = x + dx, y + dy
                if 0 <= nx <= max_x and 0 <= ny <= max_y and (nx, ny) not in blocked and (nx, ny) not in seen:
                    if (nx, ny) == goal_e:
                        return d + 1
                    seen.add((nx, ny))
                    q.append((nx, ny, d + 1))
        return None

    # Llevar el hueco justo delante de (max_x, 0) -> (max_x - 1, 0)
    steps = bfs_empty(empty_start, (max_x - 1, 0), walls)
    goal_pos = (max_x, 0)
    hole_pos = (max_x - 1, 0)

    while goal_pos != (0, 0):
        # Intercambiar hueco y dato objetivo: +1 paso
        steps += 1
        hole_pos, goal_pos = goal_pos, hole_pos
        if goal_pos == (0, 0):
            break
        # Mover hueco alrededor para situarse delante de goal_pos -> (goal_pos[0] - 1, 0)
        next_hole = (goal_pos[0] - 1, 0)
        steps += bfs_empty(hole_pos, next_hole, walls | {goal_pos})
        hole_pos = next_hole

    return viable, steps


if __name__ == "__main__":
    lines = Path("input_22.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 22 -> Parte 1: {p1} | Parte 2: {p2}")
