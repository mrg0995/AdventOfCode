from collections import deque
from pathlib import Path

def simulate(lines: list[str], elf_atk: int = 3, stop_on_elf_death: bool = False):
  walls: set[tuple[int, int]] = set()
  units: list[list] = []
  uid = 0

  for y, line in enumerate(lines):
    for x, ch in enumerate(line):
      if ch == "#":
        walls.add((x, y))
      elif ch in ("E", "G"):
        units.append([uid, ch, x, y, 200])
        uid += 1

  def occupied(exclude_id: int | None = None) -> set[tuple[int, int]]:
    occ = set(walls)
    for u in units:
      if u[4] > 0 and u[0] != exclude_id:
        occ.add((u[2], u[3]))
    return occ

  def adj4(x: int, y: int) -> list[tuple[int, int]]:
    return [(x, y - 1), (x - 1, y), (x + 1, y), (x, y + 1)]

  rounds = 0
  while True:
    units.sort(key=lambda u: (u[3], u[2]))
    for u in units:
      if u[4] <= 0:
        continue
      uid_c, kind, x, y, _ = u
      enemies = [e for e in units if e[4] > 0 and e[1] != kind]
      if not enemies:
        rem_hp = sum(e[4] for e in units if e[4] > 0)
        return True, rounds * rem_hp

      in_range_enemy = [
          e for e in enemies if (e[2], e[3]) in adj4(x, y)
      ]
      if not in_range_enemy:
        occ = occupied(uid_c)
        targets = {
            adj
            for e in enemies
            for adj in adj4(e[2], e[3])
            if adj not in occ
        }
        if targets:
          q = deque([(x, y, 0)])
          visited = {(x, y): (0, None)}
          best_dist: int | None = None
          reachable: list[tuple[int, int]] = []

          while q:
            cx, cy, d = q.popleft()
            if best_dist is not None and d > best_dist:
              break
            if (cx, cy) in targets:
              best_dist = d
              reachable.append((cx, cy))
              continue
            for nx, ny in adj4(cx, cy):
              if (nx, ny) not in occ and (nx, ny) not in visited:
                visited[(nx, ny)] = (d + 1, (cx, cy))
                q.append((nx, ny, d + 1))

          if reachable:
            goal = min(reachable, key=lambda p: (p[1], p[0]))
            cur = goal
            while visited[cur][1] is not None and visited[cur][1] != (x, y):
              cur = visited[cur][1]
            u[2], u[3] = cur
            x, y = cur

      adj_enemies = [
          e for e in units if e[4] > 0 and e[1] != kind and (e[2], e[3]) in adj4(x, y)
      ]
      if adj_enemies:
        target_e = min(adj_enemies, key=lambda e: (e[4], e[3], e[2]))
        dmg = elf_atk if kind == "E" else 3
        target_e[4] -= dmg
        if target_e[4] <= 0:
          if stop_on_elf_death and target_e[1] == "E":
            return False, 0

    units = [u for u in units if u[4] > 0]
    rounds += 1

def solve(text: str) -> tuple[int, int]:
  lines = [l.strip() for l in text.strip().splitlines() if l.strip()]
  _, part1 = simulate(lines, elf_atk=3, stop_on_elf_death=False)

  atk = 4
  while True:
    won_clean, outcome = simulate(lines, elf_atk=atk, stop_on_elf_death=True)
    if won_clean:
      return part1, outcome
    atk += 1

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 15 -> Part 1: {p1} | Part 2: {p2}")