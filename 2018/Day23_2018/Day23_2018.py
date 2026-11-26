import heapq
import re
from pathlib import Path

Nanobot = tuple[int, int, int, int]

def parse_nanobots(text: str) -> list[Nanobot]:
    bots: list[Nanobot] = []
    for line in text.strip().splitlines():
        if not line.strip():
            continue
        x, y, z, r = map(int, re.findall(r"-?\d+", line))
        bots.append((x, y, z, r))
    return bots

def manhattan(a: tuple[int, int, int], b: tuple[int, int, int]) -> int:
    return abs(a[0] - b[0]) + abs(a[1] - b[1]) + abs(a[2] - b[2])

def part1_strongest_coverage(bots: list[Nanobot]) -> int:
    strongest = max(bots, key=lambda b: b[3])
    sx, sy, sz, sr = strongest
    return sum(
        1 for x, y, z, _ in bots if manhattan((sx, sy, sz), (x, y, z)) <= sr
    )

def box_bot_overlap_count(
    x0: int, x1: int, y0: int, y1: int, z0: int, z1: int, bots: list[Nanobot]
) -> int:
    count = 0
    for bx, by, bz, br in bots:
        dx = 0 if x0 <= bx <= x1 else min(abs(bx - x0), abs(bx - x1))
        dy = 0 if y0 <= by <= y1 else min(abs(by - y0), abs(by - y1))
        dz = 0 if z0 <= bz <= z1 else min(abs(bz - z0), abs(bz - z1))
        if dx + dy + dz <= br:
            count += 1
    return count

def min_dist_box_to_origin(
    x0: int, x1: int, y0: int, y1: int, z0: int, z1: int
) -> int:
    dx = 0 if x0 <= 0 <= x1 else min(abs(x0), abs(x1))
    dy = 0 if y0 <= 0 <= y1 else min(abs(y0), abs(y1))
    dz = 0 if z0 <= 0 <= z1 else min(abs(z0), abs(z1))
    return dx + dy + dz

def part2_best_point_distance(bots: list[Nanobot]) -> int:
    min_x = min(b[0] - b[3] for b in bots)
    max_x = max(b[0] + b[3] for b in bots)
    min_y = min(b[1] - b[3] for b in bots)
    max_y = max(b[1] + b[3] for b in bots)
    min_z = min(b[2] - b[3] for b in bots)
    max_z = max(b[2] + b[3] for b in bots)

    span = max(max_x - min_x, max_y - min_y, max_z - min_z)
    size = 1
    while size < span:
        size <<= 1

    initial_count = box_bot_overlap_count(
        min_x, min_x + size, min_y, min_y + size, min_z, min_z + size, bots
    )
    initial_dist = min_dist_box_to_origin(
        min_x, min_x + size, min_y, min_y + size, min_z, min_z + size
    )
    heap = [(-initial_count, initial_dist, size, min_x, min_y, min_z)]

    while heap:
        neg_cnt, dist0, sz, x0, y0, z0 = heapq.heappop(heap)
        if sz == 0:
            return dist0

        half = sz >> 1
        for dx in (0, half):
            for dy in (0, half):
                for dz in (0, half):
                    nx0, ny0, nz0 = x0 + dx, y0 + dy, z0 + dz
                    nx1, ny1, nz1 = nx0 + half, ny0 + half, nz0 + half
                    cnt = box_bot_overlap_count(
                        nx0, nx1, ny0, ny1, nz0, nz1, bots
                    )
                    if cnt > 0:
                        d0 = min_dist_box_to_origin(
                            nx0, nx1, ny0, ny1, nz0, nz1
                        )
                        heapq.heappush(heap, (-cnt, d0, half, nx0, ny0, nz0))

    return 0

def solve(raw_input: str):
    bots = parse_nanobots(raw_input)
    return part1_strongest_coverage(bots), part2_best_point_distance(bots)

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 23 -> Part 1: {p1} | Part 2: {p2}")