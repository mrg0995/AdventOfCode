from collections import Counter
from itertools import combinations
from pathlib import Path

def solve(text: str) -> tuple[int, str]:
  ids = [line.strip() for line in text.strip().splitlines() if line.strip()]
  twos = sum(2 in Counter(box).values() for box in ids)
  threes = sum(3 in Counter(box).values() for box in ids)
  part1 = twos * threes

  part2 = ""
  for a, b in combinations(ids, 2):
    common = [ca for ca, cb in zip(a, b) if ca == cb]
    if len(common) == len(a) - 1:
      part2 = "".join(common)
      break

  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 2 -> Part 1: {p1} | Part 2: {p2}")