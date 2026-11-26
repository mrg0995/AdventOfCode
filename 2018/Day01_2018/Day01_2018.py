from pathlib import Path

def solve(text: str) -> tuple[int, int]:
  deltas = [int(line) for line in text.strip().splitlines() if line.strip()]
  part1 = sum(deltas)

  freq = 0
  seen = {0}
  part2 = None

  while part2 is None:
    for d in deltas:
      freq += d
      if freq in seen:
        part2 = freq
        break
      seen.add(freq)

  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Part 1: {p1} | Part 2: {p2}")
