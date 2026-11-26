from functools import cache
import heapq
from pathlib import Path
import re

def solve(text: str) -> tuple[int, int]:
  nums = list(map(int, re.findall(r"\d+", text)))
  depth, tx, ty = nums[0], nums[1], nums[2]

  @cache
  def geo_index(x: int, y: int) -> int:
    if (x == 0 and y == 0) or (x == tx and y == ty):
      return 0
    if y == 0:
      return x * 16807
    if x == 0:
      return y * 48271
    return erosion(x - 1, y) * erosion(x, y - 1)

  @cache
  def erosion(x: int, y: int) -> int:
    return (geo_index(x, y) + depth) % 20183

  def region_type(x: int, y: int) -> int:
    return erosion(x, y) % 3

  part1 = sum(region_type(x, y) for x in range(tx + 1) for y in range(ty + 1))

  allowed = {
      0: {1, 2},
      1: {1, 0},
      2: {2, 0},
  }

  pq = [(0, 0, 0, 2)]
  best: dict[tuple[int, int, int], int] = {}
  max_x, max_y = tx + 60, ty + 60

  while pq:
    minutes, x, y, tool = heapq.heappop(pq)
    if (x, y, tool) == (tx, ty, 2):
      return part1, minutes
    if best.get((x, y, tool), 10**9) <= minutes:
      continue
    best[(x, y, tool)] = minutes

    cur_type = region_type(x, y)
    for other_tool in allowed[cur_type]:
      if other_tool != tool:
        heapq.heappush(pq, (minutes + 7, x, y, other_tool))

    for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
      nx, ny = x + dx, y + dy
      if 0 <= nx <= max_x and 0 <= ny <= max_y:
        nxt_type = region_type(nx, ny)
        if tool in allowed[nxt_type]:
          heapq.heappush(pq, (minutes + 1, nx, ny, tool))

  return part1, -1

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 22 -> Part 1: {p1} | Part 2: {p2}")