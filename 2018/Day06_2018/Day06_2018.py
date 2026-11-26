from collections import Counter
from pathlib import Path

def solve(text: str) -> tuple[int, int]:
  pts = [
      tuple(map(int, line.split(", ")))
      for line in text.strip().splitlines()
      if line.strip()
  ]
  min_x, max_x = min(x for x, _ in pts), max(x for x, _ in pts)
  min_y, max_y = min(y for _, y in pts), max(y for _, y in pts)

  areas: Counter[int] = Counter()
  infinite: set[int] = set()
  safe_region = 0

  for x in range(min_x, max_x + 1):
    for y in range(min_y, max_y + 1):
      dists = [(abs(x - px) + abs(y - py), i) for i, (px, py) in enumerate(pts)]
      if sum(d for d, _ in dists) < 10_000:
        safe_region += 1

      dists.sort()
      if dists[0][0] < dists[1][0]:
        owner = dists[0][1]
        areas[owner] += 1
        if x in (min_x, max_x) or y in (min_y, max_y):
          infinite.add(owner)

  part1 = max(area for idx, area in areas.items() if idx not in infinite)
  return part1, safe_region

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 6 -> Part 1: {p1} | Part 2: {p2}")