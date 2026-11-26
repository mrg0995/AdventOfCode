from collections import defaultdict
from pathlib import Path
import re

def solve(text: str) -> tuple[int, int]:
  pattern = re.compile(r"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")
  claims = [
      tuple(map(int, pattern.match(line).groups()))
      for line in text.strip().splitlines()
      if line.strip()
  ]

  fabric: dict[tuple[int, int], int] = defaultdict(int)
  for _, x, y, w, h in claims:
    for dx in range(w):
      for dy in range(h):
        fabric[(x + dx, y + dy)] += 1

  part1 = sum(v >= 2 for v in fabric.values())

  part2 = next(
      cid
      for cid, x, y, w, h in claims
      if all(
          fabric[(x + dx, y + dy)] == 1 for dx in range(w) for dy in range(h)
      )
  )
  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 3 -> Part 1: {p1} | Part 2: {p2}")