from collections import deque
import heapq
from pathlib import Path

def reachable_keys_from(
    grid: list[str], start: tuple[int, int]
) -> dict[str, tuple[int, int]]:
    h, w = len(grid), len(grid[0])
    sx, sy = start
    queue: deque[tuple[int, int, int, int]] = deque([(sx, sy, 0, 0)])
    visited: set[tuple[int, int]] = {(sx, sy)}
    dirs = ((-1, 0), (1, 0), (0, -1), (0, 1))
    found: dict[str, tuple[int, int]] = {}

    while queue:
        x, y, dist, doors_mask = queue.popleft()
        for dx, dy in dirs:
            nx, ny = x + dx, y + dy
            if not (0 <= ny < h and 0 <= nx < w):
                continue
            if (nx, ny) in visited:
                continue

            ch = grid[ny][nx]
            if ch == "#":
                continue

            visited.add((nx, ny))
            next_doors = doors_mask
            if "A" <= ch <= "Z":
                next_doors |= 1 << (ord(ch) - ord("A"))

            next_dist = dist + 1
            if "a" <= ch <= "z":
                if ch not in found:
                    found[ch] = (next_dist, next_doors)

            queue.append((nx, ny, next_dist, next_doors))

    return found

def shortest_path_keys(
    grid: list[str], starts: list[tuple[int, int]], all_mask: int
) -> int:
    if all_mask == 0:
        return 0

    node_coords: dict[str, tuple[int, int]] = {}
    start_ids: list[str] = []
    for idx, pos in enumerate(starts):
        nid = str(idx)
        node_coords[nid] = pos
        start_ids.append(nid)

    h, w = len(grid), len(grid[0])
    for y in range(h):
        for x in range(w):
            ch = grid[y][x]
            if "a" <= ch <= "z":
                node_coords[ch] = (x, y)

    edges: dict[str, dict[str, tuple[int, int]]] = {}
    for nid, pos in node_coords.items():
        found = reachable_keys_from(grid, pos)
        edges[nid] = {
            target_key: (dist, req_mask)
            for target_key, (dist, req_mask) in found.items()
            if target_key != nid
        }

    init_state = (tuple(start_ids), 0)
    pq: list[tuple[int, tuple[str, ...], int]] = [(0, tuple(start_ids), 0)]
    best_dist: dict[tuple[tuple[str, ...], int], int] = {init_state: 0}

    while pq:
        cost, positions, mask = heapq.heappop(pq)
        if mask == all_mask:
            return cost

        if best_dist.get((positions, mask), float("inf")) < cost:
            continue

        for r_idx, cur_node in enumerate(positions):
            for nxt_key, (step_dist, req_doors) in edges[cur_node].items():
                key_bit = 1 << (ord(nxt_key) - ord("a"))
                if mask & key_bit:
                    continue
                if (mask & req_doors) != req_doors:
                    continue

                new_mask = mask | key_bit
                new_pos = positions[:r_idx] + (nxt_key,) + positions[r_idx + 1 :]
                new_cost = cost + step_dist
                state = (new_pos, new_mask)

                if new_cost < best_dist.get(state, float("inf")):
                    best_dist[state] = new_cost
                    heapq.heappush(pq, (new_cost, new_pos, new_mask))

    return -1

def solve(lines: list[str]) -> tuple[int, int]:
    grid = [line.rstrip("\r\n") for line in lines if line.strip("\r\n")]
    if not grid or not grid[0]:
        raise ValueError("El mapa está vacío.")

    h, w = len(grid), len(grid[0])
    all_mask = 0
    center: tuple[int, int] | None = None

    for y in range(h):
        for x in range(w):
            ch = grid[y][x]
            if ch == "@":
                center = (x, y)
            elif "a" <= ch <= "z":
                all_mask |= 1 << (ord(ch) - ord("a"))

    if center is None:
        raise ValueError("No se encontró la posición inicial '@'.")

    cx, cy = center
    if not (1 <= cx < w - 1 and 1 <= cy < h - 1):
        raise ValueError("La posición '@' está demasiado cerca del borde para el parche 3x3.")

    p1 = shortest_path_keys(grid, [center], all_mask)

    mut = [list(row) for row in grid]
    patch = (
        ("@", "#", "@"),
        ("#", "#", "#"),
        ("@", "#", "@"),
    )
    for dy in (-1, 0, 1):
        for dx in (-1, 0, 1):
            mut[cy + dy][cx + dx] = patch[dy + 1][dx + 1]

    robots = [
        (cx - 1, cy - 1),
        (cx + 1, cy - 1),
        (cx - 1, cy + 1),
        (cx + 1, cy + 1),
    ]
    p2 = shortest_path_keys(["".join(r) for r in mut], robots, all_mask)
    return p1, p2

if __name__ == "__main__":
    input_path = Path("Day18_2019.txt")
    lines = input_path.read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 18 - Part 1: {p1}, Part 2: {p2}")