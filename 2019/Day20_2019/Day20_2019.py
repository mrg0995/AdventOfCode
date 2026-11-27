from collections import defaultdict, deque
from pathlib import Path

def solve(lines: list[str]) -> tuple[int, int]:
    grid = [line.rstrip("\r\n") for line in lines if line.strip("\r\n")]
    h = len(grid)
    w = max(len(r) for r in grid)
    padded = [r.ljust(w, " ") for r in grid]

    open_tiles: set[tuple[int, int]] = set()
    for y in range(h):
        for x in range(w):
            if padded[y][x] == ".":
                open_tiles.add((x, y))

    raw_portals: dict[str, list[tuple[int, int]]] = defaultdict(list)
    tile_portal: dict[tuple[int, int], str] = {}

    for y in range(h):
        for x in range(w):
            ch = padded[y][x]
            if not ("A" <= ch <= "Z"):
                continue
            if x + 1 < w and "A" <= padded[y][x + 1] <= "Z":
                code = ch + padded[y][x + 1]
                if x - 1 >= 0 and padded[y][x - 1] == ".":
                    pt = (x - 1, y)
                    raw_portals[code].append(pt)
                    tile_portal[pt] = code
                elif x + 2 < w and padded[y][x + 2] == ".":
                    pt = (x + 2, y)
                    raw_portals[code].append(pt)
                    tile_portal[pt] = code
            if y + 1 < h and "A" <= padded[y + 1][x] <= "Z":
                code = ch + padded[y + 1][x]
                if y - 1 >= 0 and padded[y - 1][x] == ".":
                    pt = (x, y - 1)
                    raw_portals[code].append(pt)
                    tile_portal[pt] = code
                elif y + 2 < h and padded[y + 2][x] == ".":
                    pt = (x, y + 2)
                    raw_portals[code].append(pt)
                    tile_portal[pt] = code

    def is_outer(pt: tuple[int, int]) -> bool:
        x, y = pt
        return x <= 2 or x >= w - 3 or y <= 2 or y >= h - 3

    warp: dict[tuple[int, int], tuple[tuple[int, int], int]] = {}
    for code, pts in raw_portals.items():
        if len(pts) == 2:
            a, b = pts[0], pts[1]
            warp[a] = (b, -1 if is_outer(a) else 1)
            warp[b] = (a, -1 if is_outer(b) else 1)

    start = raw_portals["AA"][0]
    goal = raw_portals["ZZ"][0]
    dirs = ((-1, 0), (1, 0), (0, -1), (0, 1))

    q1: deque[tuple[tuple[int, int], int]] = deque([(start, 0)])
    seen1 = {start}
    p1 = -1
    while q1:
        pos, dist = q1.popleft()
        if pos == goal:
            p1 = dist
            break
        candidates = [(pos[0] + dx, pos[1] + dy) for dx, dy in dirs]
        if pos in warp:
            candidates.append(warp[pos][0])
        for nxt in candidates:
            if nxt in open_tiles and nxt not in seen1:
                seen1.add(nxt)
                q1.append((nxt, dist + 1))

    q2: deque[tuple[tuple[int, int], int, int]] = deque([(start, 0, 0)])
    seen2 = {(start, 0)}
    p2 = -1
    while q2:
        pos, level, dist = q2.popleft()
        if pos == goal and level == 0:
            p2 = dist
            break

        for dx, dy in dirs:
            nxt = (pos[0] + dx, pos[1] + dy)
            if nxt in open_tiles and (nxt, level) not in seen2:
                seen2.add((nxt, level))
                q2.append((nxt, level, dist + 1))

        if pos in warp:
            w_pos, d_lvl = warp[pos]
            n_lvl = level + d_lvl
            if n_lvl >= 0 and (w_pos, n_lvl) not in seen2:
                seen2.add((w_pos, n_lvl))
                q2.append((w_pos, n_lvl, dist + 1))

    return p1, p2

if __name__ == "__main__":
    lines = Path("Day20_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 20 - Part 1: {p1}, Part 2: {p2}")