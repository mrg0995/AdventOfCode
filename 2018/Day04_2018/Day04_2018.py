from collections import Counter, defaultdict
from pathlib import Path
import re

def solve(text: str) -> tuple[int, int]:
  lines = sorted(
      line.strip() for line in text.strip().splitlines() if line.strip()
  )
  minutes_asleep: dict[int, Counter[int]] = defaultdict(Counter)

  guard = 0
  start = 0
  for line in lines:
    minute = int(line[15:17])
    if "Guard #" in line:
      guard = int(re.search(r"#(\d+)", line).group(1))
    elif "falls asleep" in line:
      start = minute
    elif "wakes up" in line:
      minutes_asleep[guard].update(range(start, minute))

  sleepiest_guard = max(
      minutes_asleep, key=lambda g: sum(minutes_asleep[g].values())
  )
  best_min_1 = minutes_asleep[sleepiest_guard].most_common(1)[0][0]
  part1 = sleepiest_guard * best_min_1

  most_freq_guard = max(
      minutes_asleep,
      key=lambda g: minutes_asleep[g].most_common(1)[0][1]
      if minutes_asleep[g]
      else 0,
  )
  best_min_2 = minutes_asleep[most_freq_guard].most_common(1)[0][0]
  part2 = most_freq_guard * best_min_2

  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 4 -> Part 1: {p1} | Part 2: {p2}")