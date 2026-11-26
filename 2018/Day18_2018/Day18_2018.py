from collections import Counter
from pathlib import Path

def step_grid(grid: list[str]) -> list[str]:
  h, w = len(grid), len(grid[0])
  out = []
  for y in range(h):
    row = []
    for x in range(w):
      c = grid[y][x]
      cnt = Counter(
          grid[ny][nx]
          for dy in (-1, 0, 1)
          for dx in (-1, 0, 1)
          if (dx or dy) and 0 <= (nx := x + dx) < w and 0 <= (ny := y + dy) < h
      )
      if c == ".":
        row.append("|" if cnt["|"] >= 3 else ".")
      elif c == "|":
        row.append("#" if cnt["#"] >= 3 else "|")
      else:
        row.append("#" if (cnt["#"] >= 1 and cnt["|"] >= 1) else ".")
    out.append("".join(row))
  return out

def resource_value(grid: list[str]) -> int:
  flat = "".join(grid)
  return flat.count("|") * flat.count("#")

def solve(text: str) -> tuple[int, int]:
  grid = [l.strip() for l in text.strip().splitlines() if l.strip()]

  g1 = list(grid)
  for _ in range(10):
    g1 = step_grid(g1)
  part1 = resource_value(g1)

  seen: dict[tuple[str, ...], int] = {}
  g2 = list(grid)
  target = 1_000_000_000
  minute = 0

  while minute < target:
    key = tuple(g2)
    if key in seen:
      start = seen[key]
      period = minute - start
      rem = (target - minute) % period
      for _ in range(rem):
        g2 = step_grid(g2)
      break
    seen[key] = minute
    g2 = step_grid(g2)
    minute += 1

  return part1, resource_value(g2)

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 18 -> Part 1: {p1} | Part 2: {p2}")