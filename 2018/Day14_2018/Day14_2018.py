def solve(text: str) -> tuple[str, int]:
  n = int(text.strip())
  target = [int(ch) for ch in str(n)]
  scores = [3, 7]
  e1, e2 = 0, 1
  part2: int | None = None

  while len(scores) < n + 10 or part2 is None:
    s = scores[e1] + scores[e2]
    if s >= 10:
      scores.append(1)
      if part2 is None and scores[-len(target) :] == target:
        part2 = len(scores) - len(target)
      scores.append(s % 10)
    else:
      scores.append(s)

    if part2 is None and len(scores) >= len(target) and scores[-len(target) :] == target:
      part2 = len(scores) - len(target)

    e1 = (e1 + 1 + scores[e1]) % len(scores)
    e2 = (e2 + 1 + scores[e2]) % len(scores)

  part1 = "".join(str(x) for x in scores[n : n + 10])
  return part1, part2

puzzle_input = "825401"
p1, p2 = solve(puzzle_input)
print(f"Day 14 -> Part 1: {p1} | Part 2: {p2}")