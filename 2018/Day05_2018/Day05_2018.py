from pathlib import Path

def react(polymer: str) -> int:
  stack: list[str] = []
  for ch in polymer:
    if stack and stack[-1] != ch and stack[-1].lower() == ch.lower():
      stack.pop()
    else:
      stack.append(ch)
  return len(stack)

def solve(text: str) -> tuple[int, int]:
  polymer = text.strip()
  part1 = react(polymer)
  part2 = min(
      react(polymer.replace(chr(c), "").replace(chr(c).upper(), ""))
      for c in range(ord("a"), ord("z") + 1)
  )
  return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 5 -> Part 1: {p1} | Part 2: {p2}")