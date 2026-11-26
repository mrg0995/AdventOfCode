from collections import defaultdict
from pathlib import Path

DELTAS = {
    "N": (0, -1),
    "S": (0, 1),
    "E": (1, 0),
    "W": (-1, 0),
}

def build_distances(regex: str) -> dict[tuple[int, int], int]:
    pattern = regex.strip().removeprefix("^").removesuffix("$")
    dist = defaultdict(lambda: float("inf"))
    start = (0, 0)
    dist[start] = 0

    current_positions = {start}
    stack: list[tuple[set[tuple[int, int]], set[tuple[int, int]]]] = []

    for ch in pattern:
        if ch in DELTAS:
            dx, dy = DELTAS[ch]
            next_positions = set()
            for x, y in current_positions:
                nx, ny = x + dx, y + dy
                new_d = dist[(x, y)] + 1
                if new_d < dist[(nx, ny)]:
                    dist[(nx, ny)] = new_d
                next_positions.add((nx, ny))
            current_positions = next_positions
        elif ch == "(":
            stack.append((current_positions.copy(), set()))
        elif ch == "|":
            starts, branch_ends = stack[-1]
            branch_ends |= current_positions
            current_positions = starts.copy()
        elif ch == ")":
            starts, branch_ends = stack.pop()
            branch_ends |= current_positions
            current_positions = branch_ends

    return dist

def solve(raw_input: str):
    distances = build_distances(raw_input)
    reachable = [d for d in distances.values() if d != float("inf")]
    part1 = max(reachable)
    part2 = sum(1 for d in reachable if d >= 1000)
    return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 20 -> Part 1: {p1} | Part 2: {p2}")