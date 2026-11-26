from collections import defaultdict
import heapq
from pathlib import Path

def parse(text: str):
  prereqs: dict[str, set[str]] = defaultdict(set)
  dependents: dict[str, list[str]] = defaultdict(list)
  steps: set[str] = set()
  for line in text.strip().splitlines():
    if not line.strip():
      continue
    parts = line.split()
    a, b = parts[1], parts[7]
    steps.update((a, b))
    prereqs[b].add(a)
    dependents[a].append(b)
  return steps, prereqs, dependents

def solve(text: str) -> tuple[str, int]:
  steps, prereqs, dependents = parse(text)
  avail = [s for s in steps if not prereqs[s]]
  heapq.heapify(avail)
  order: list[str] = []
  while avail:
    curr = heapq.heappop(avail)
    order.append(curr)
    for nxt in dependents[curr]:
      prereqs[nxt].remove(curr)
      if not prereqs[nxt]:
        heapq.heappush(avail, nxt)

  steps, prereqs, dependents = parse(text)
  avail = [s for s in steps if not prereqs[s]]
  heapq.heapify(avail)
  workers: list[tuple[int, str]] = []
  time = 0
  done = 0

  while done < len(steps):
    while avail and len(workers) < 5:
      step = heapq.heappop(avail)
      finish = time + 60 + (ord(step) - ord("A") + 1)
      heapq.heappush(workers, (finish, step))

    time, finished_step = heapq.heappop(workers)
    done += 1
    for nxt in dependents[finished_step]:
      prereqs[nxt].remove(finished_step)
      if not prereqs[nxt]:
        heapq.heappush(avail, nxt)

  return "".join(order), time

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 7 -> Part 1: {p1} | Part 2: {p2}")