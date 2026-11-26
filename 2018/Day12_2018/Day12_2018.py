from pathlib import Path

def solve(text: str) -> tuple[int, int]:
  lines = [l.strip() for l in text.strip().splitlines() if l.strip()]
  state = {i for i, ch in enumerate(lines[0].split(": ")[1]) if ch == "#"}
  rules = {}
  for line in lines[1:]:
    pat, out = line.split(" => ")
    rules[pat] = out == "#"

  def step(cur: set[int]) -> set[int]:
    lo, hi = min(cur) - 2, max(cur) + 2
    nxt = set()
    for pos in range(lo, hi + 1):
      pat = "".join("#" if (pos + d) in cur else "." for d in range(-2, 3))
      if rules.get(pat, False):
        nxt.add(pos)
    return nxt

  cur = set(state)
  for _ in range(20):
    cur = step(cur)
  part1 = sum(cur)

  cur = set(state)
  prev_sum = sum(cur)
  prev_diff = None
  stable_count = 0
  gen = 0
  target = 50_000_000_000

  while gen < target:
    gen += 1
    cur = step(cur)
    s = sum(cur)
    diff = s - prev_sum
    if diff == prev_diff:
      stable_count += 1
      if stable_count >= 10:
        part2 = s + (target - gen) * diff
        break
    else:
      stable_count = 0
      prev_diff = diff
    prev_sum = s

  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 12 -> Part 1: {p1} | Part 2: {p2}")