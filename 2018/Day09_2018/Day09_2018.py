from collections import deque
from pathlib import Path

def play(players: int, last_marble: int) -> int:
  scores = [0] * players
  circle = deque([0])

  for marble in range(1, last_marble + 1):
    if marble % 23 == 0:
      circle.rotate(7)
      scores[marble % players] += marble + circle.pop()
      circle.rotate(-1)
    else:
      circle.rotate(-1)
      circle.append(marble)

  return max(scores)

def solve(text: str) -> tuple[int, int]:
  parts = text.strip().split()
  players, last_marble = int(parts[0]), int(parts[6])
  return play(players, last_marble), play(players, last_marble * 100)

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 9 -> Part 1: {p1} | Part 2: {p2}")