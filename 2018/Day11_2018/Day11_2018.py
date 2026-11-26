def power_level(x: int, y: int, serial: int) -> int:
  rack_id = x + 10
  p = (rack_id * y + serial) * rack_id
  return (p // 100) % 10 - 5

def solve(text: str) -> tuple[str, str]:
  serial = int(text.strip())
  size = 300
  sat = [[0] * (size + 1) for _ in range(size + 1)]
  for y in range(1, size + 1):
    row_sum = 0
    for x in range(1, size + 1):
      row_sum += power_level(x, y, serial)
      sat[y][x] = sat[y - 1][x] + row_sum

  def rect_sum(x1: int, y1: int, s: int) -> int:
    x2, y2 = x1 + s - 1, y1 + s - 1
    return sat[y2][x2] - sat[y1 - 1][x2] - sat[y2][x1 - 1] + sat[y1 - 1][x1 - 1]

  best3 = (-10**9, 0, 0)
  best_any = (-10**9, 0, 0, 0)

  for s in range(1, size + 1):
    for y in range(1, size - s + 2):
      for x in range(1, size - s + 2):
        total = rect_sum(x, y, s)
        if s == 3 and total > best3[0]:
          best3 = (total, x, y)
        if total > best_any[0]:
          best_any = (total, x, y, s)

  return f"{best3[1]},{best3[2]}", f"{best_any[1]},{best_any[2]},{best_any[3]}"

puzzle_input = "7347"
p1, p2 = solve(puzzle_input)
print(f"Day 11 -> Part 1: {p1} | Part 2: {p2}")