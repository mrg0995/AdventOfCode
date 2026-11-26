from pathlib import Path
import re

def solve(text: str) -> tuple[str, int]:
  rx = re.compile(r"position=<\s*(-?\d+),\s*(-?\d+)> velocity=<\s*(-?\d+),\s*(-?\d+)>")
  stars = [
      tuple(map(int, rx.match(line).groups()))
      for line in text.strip().splitlines()
      if line.strip()
  ]

  t = 0
  prev_area = float("inf")
  while True:
    xs = [x + vx * t for x, _, vx, _ in stars]
    ys = [y + vy * t for _, y, _, vy in stars]
    area = (max(xs) - min(xs)) * (max(ys) - min(ys))
    if area > prev_area:
      t -= 1
      break
    prev_area = area
    t += 1

  pts = {(x + vx * t, y + vy * t) for x, y, vx, vy in stars}
  min_x, max_x = min(x for x, _ in pts), max(x for x, _ in pts)
  min_y, max_y = min(y for _, y in pts), max(y for _, y in pts)

  banner = "\n".join(
      "".join("#" if (x, y) in pts else "." for x in range(min_x, max_x + 1))
      for y in range(min_y, max_y + 1)
  )
  return banner, t

msg, seconds = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 10 -> Part 2: {seconds}s\nPart 1:\n{msg}")