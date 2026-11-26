from pathlib import Path
import re
import sys

sys.setrecursionlimit(20000)

def solve(text: str) -> tuple[int, int]:
  clay: set[tuple[int, int]] = set()
  for line in text.strip().splitlines():
    if not line.strip():
      continue
    a, b, c = map(int, re.findall(r"\d+", line))
    if line.startswith("x="):
      for y in range(b, c + 1):
        clay.add((a, y))
    else:
      for x in range(b, c + 1):
        clay.add((x, a))

  min_y = min(y for _, y in clay)
  max_y = max(y for _, y in clay)

  settled: set[tuple[int, int]] = set()
  flowing: set[tuple[int, int]] = set()

  def flow(x: int, y: int) -> bool:
    if y > max_y:
      return False
    if (x, y) in clay or (x, y) in settled:
      return True
    if (x, y) in flowing:
      return False

    flowing.add((x, y))
    if not flow(x, y + 1):
      return False

    left_x = x
    left_bounded = False
    while True:
      if (left_x - 1, y) in clay:
        left_bounded = True
        break
      flowing.add((left_x - 1, y))
      if not flow(left_x - 1, y + 1):
        break
      left_x -= 1

    right_x = x
    right_bounded = False
    while True:
      if (right_x + 1, y) in clay:
        right_bounded = True
        break
      flowing.add((right_x + 1, y))
      if not flow(right_x + 1, y + 1):
        break
      right_x += 1

    if left_bounded and right_bounded:
      for wx in range(left_x, right_x + 1):
        flowing.discard((wx, y))
        settled.add((wx, y))
      return True

    return False

  flow(500, min_y)
  in_range_settled = {p for p in settled if min_y <= p[1] <= max_y}
  in_range_flowing = {p for p in flowing if min_y <= p[1] <= max_y}
  return len(in_range_settled | in_range_flowing), len(in_range_settled)

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 17 -> Part 1: {p1} | Part 2: {p2}")