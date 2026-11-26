from collections import deque
from pathlib import Path
import re

def solve(text: str) -> int:
  pts = [
      tuple(map(int, re.findall(r"-?\d+", line)))
      for line in text.strip().splitlines()
      if line.strip()
  ]

  def dist4(a: tuple[int, ...], b: tuple[int, ...]) -> int:
    return sum(abs(x - y) for x, y in zip(a, b))

  n = len(pts)
  visited = [False] * n
  constellations = 0

  for i in range(n):
    if visited[i]:
      continue
    constellations += 1
    q = deque([i])
    visited[i] = True
    while q:
      curr = q.popleft()
      for nxt in range(n):
        if not visited[nxt] and dist4(pts[curr], pts[nxt]) <= 3:
          visited[nxt] = True
          q.append(nxt)

  return constellations

ans = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 25 -> Constellations: {ans}")