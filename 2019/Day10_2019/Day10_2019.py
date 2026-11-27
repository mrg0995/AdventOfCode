from collections import defaultdict, deque
from math import atan2, hypot, pi
from pathlib import Path

def clockwise_angle(dx: int, dy: int) -> float:
    ang = atan2(dx, -dy)
    return ang if ang >= 0 else ang + 2 * pi

def solve(lines: list[str]) -> tuple[int, int]:
    asteroids: list[tuple[int, int]] = []
    for y, line in enumerate(lines):
        row = line.rstrip("\r\n")
        for x, ch in enumerate(row):
            if ch == "#":
                asteroids.append((x, y))

    best_pos = (0, 0)
    best_count = -1

    for ax, ay in asteroids:
        angles = {clockwise_angle(bx - ax, by - ay) for bx, by in asteroids if (bx, by) != (ax, ay)}
        if len(angles) > best_count:
            best_count = len(angles)
            best_pos = (ax, ay)

    sx, sy = best_pos
    by_angle: dict[float, list[tuple[float, int, int]]] = defaultdict(list)
    for bx, by in asteroids:
        if (bx, by) == (sx, sy):
            continue
        dx, dy = bx - sx, by - sy
        ang = clockwise_angle(dx, dy)
        dist = hypot(dx, dy)
        by_angle[ang].append((dist, bx, by))

    ordered_angles = sorted(by_angle.keys())
    queues: list[deque[tuple[int, int]]] = []
    for ang in ordered_angles:
        items = sorted(by_angle[ang], key=lambda t: t[0])
        queues.append(deque((x, y) for _, x, y in items))

    vaporized = 0
    target_pos = (0, 0)
    while vaporized < 200 and any(queues):
        for q in queues:
            if q:
                pos = q.popleft()
                vaporized += 1
                if vaporized == 200:
                    target_pos = pos
                    break

    p2 = target_pos[0] * 100 + target_pos[1]
    return best_count, p2

if __name__ == "__main__":
    lines = [l for l in Path("Day10_2019.txt").read_text(encoding="utf-8").splitlines() if l.strip()]
    p1, p2 = solve(lines)
    print(f"Day 10 - Part 1: {p1}, Part 2: {p2}")