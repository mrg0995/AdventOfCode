from pathlib import Path
import re

def build_ops():
  return {
      "addr": lambda r, a, b: r[a] + r[b],
      "addi": lambda r, a, b: r[a] + b,
      "mulr": lambda r, a, b: r[a] * r[b],
      "muli": lambda r, a, b: r[a] * b,
      "banr": lambda r, a, b: r[a] & r[b],
      "bani": lambda r, a, b: r[a] & b,
      "borr": lambda r, a, b: r[a] | r[b],
      "bori": lambda r, a, b: r[a] | b,
      "setr": lambda r, a, b: r[a],
      "seti": lambda r, a, b: a,
      "gtir": lambda r, a, b: 1 if a > r[b] else 0,
      "gtri": lambda r, a, b: 1 if r[a] > b else 0,
      "gtrr": lambda r, a, b: 1 if r[a] > r[b] else 0,
      "eqir": lambda r, a, b: 1 if a == r[b] else 0,
      "eqri": lambda r, a, b: 1 if r[a] == b else 0,
      "eqrr": lambda r, a, b: 1 if r[a] == r[b] else 0,
  }

def solve(text: str) -> tuple[int, int]:
  ops = build_ops()
  parts = text.strip().split("\n\n\n")
  sample_blocks = parts[0].strip().split("\n\n")
  program_lines = (
      [l.strip() for l in parts[1].strip().splitlines() if l.strip()]
      if len(parts) > 1
      else []
  )

  three_or_more = 0
  candidates: dict[int, set[str]] = {i: set(ops.keys()) for i in range(16)}

  for block in sample_blocks:
    nums = [int(x) for x in re.findall(r"-?\d+", block)]
    before, inst, after = nums[0:4], nums[4:8], nums[8:12]
    opcode, a, b, c = inst

    matches = set()
    for name, fn in ops.items():
      r = list(before)
      r[c] = fn(r, a, b)
      if r == after:
        matches.add(name)

    if len(matches) >= 3:
      three_or_more += 1
    candidates[opcode] &= matches

  op_map: dict[int, str] = {}
  while len(op_map) < 16:
    for code, s in candidates.items():
      if len(s) == 1:
        name = next(iter(s))
        op_map[code] = name
        for other in candidates.values():
          other.discard(name)

  regs = [0, 0, 0, 0]
  for line in program_lines:
    op, a, b, c = map(int, line.split())
    regs[c] = ops[op_map[op]](regs, a, b)

  return three_or_more, regs[0]

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 16 -> Part 1: {p1} | Part 2: {p2}")