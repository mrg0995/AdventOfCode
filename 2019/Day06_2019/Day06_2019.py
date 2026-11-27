from collections import deque
from pathlib import Path

def solve(lines: list[str]) -> tuple[int, int]:
    parent: dict[str, str] = {}
    adj: dict[str, list[str]] = {}

    for line in lines:
        line = line.strip()
        if not line:
            continue
        center, orbiter = line.split(")")
        parent[orbiter] = center
        adj.setdefault(center, []).append(orbiter)
        adj.setdefault(orbiter, []).append(center)

    total_orbits = 0
    for node in parent:
        curr = node
        while curr in parent:
            total_orbits += 1
            curr = parent[curr]

    start = parent["YOU"]
    goal = parent["SAN"]
    queue = deque([(start, 0)])
    visited = {start}
    transfers = -1

    while queue:
        curr, dist = queue.popleft()
        if curr == goal:
            transfers = dist
            break
        for nxt in adj.get(curr, []):
            if nxt not in visited:
                visited.add(nxt)
                queue.append((nxt, dist + 1))

    return total_orbits, transfers

if __name__ == "__main__":
    lines = Path("Day06_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 6 - Part 1: {p1}, Part 2: {p2}")