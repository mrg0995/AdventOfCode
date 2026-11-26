from pathlib import Path

def solve(text: str) -> tuple[str, str]:
  raw_lines = text.rstrip("\n").splitlines()
  grid: dict[tuple[int, int], str] = {}
  carts: list[list[int]] = []
  dirs = {"^": (0, -1), "v": (0, 1), "<": (-1, 0), ">": (1, 0)}

  for y, line in enumerate(raw_lines):
    for x, ch in enumerate(line):
      if ch in dirs:
        dx, dy = dirs[ch]
        carts.append([x, y, dx, dy, 0])
        grid[(x, y)] = "|" if dy != 0 else "-"
      elif ch in ("/", "\\", "+", "-", "|"):
        grid[(x, y)] = ch

  def turn_left(dx: int, dy: int) -> tuple[int, int]:
    return (dy, -dx)

  def turn_right(dx: int, dy: int) -> tuple[int, int]:
    return (-dy, dx)

  first_crash: tuple[int, int] | None = None
  active = [list(c) for c in carts]

  while True:
    active.sort(key=lambda c: (c[1], c[0]))
    crashed_indices: set[int] = set()

    for i, cart in enumerate(active):
      if i in crashed_indices:
        continue
      cart[0] += cart[2]
      cart[1] += cart[3]
      x, y, dx, dy, t_idx = cart

      for j, other in enumerate(active):
        if i != j and j not in crashed_indices and other[0] == x and other[1] == y:
          if first_crash is None:
            first_crash = (x, y)
          crashed_indices.update((i, j))

      if i in crashed_indices:
        continue

      track = grid.get((x, y), "")
      if track == "/":
        dx, dy = (-dy, -dx)
      elif track == "\\":
        dx, dy = (dy, dx)
      elif track == "+":
        mode = t_idx % 3
        if mode == 0:
          dx, dy = turn_left(dx, dy)
        elif mode == 2:
          dx, dy = turn_right(dx, dy)
        cart[4] = t_idx + 1
      cart[2], cart[3] = dx, dy

    if crashed_indices:
      active = [c for idx, c in enumerate(active) if idx not in crashed_indices]

    if len(active) == 1:
      last_pos = (active[0][0], active[0][1])
      break

  return f"{first_crash[0]},{first_crash[1]}", f"{last_pos[0]},{last_pos[1]}"

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 13 -> Part 1: {p1} | Part 2: {p2}")