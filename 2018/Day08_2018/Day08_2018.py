from pathlib import Path

def solve(text: str) -> tuple[int, int]:
  nums = list(map(int, text.strip().split()))
  idx = 0

  def parse_node() -> tuple[int, int]:
    nonlocal idx
    n_children, n_meta = nums[idx], nums[idx + 1]
    idx += 2

    child_values: list[int] = []
    total_meta = 0

    for _ in range(n_children):
      c_meta, c_val = parse_node()
      total_meta += c_meta
      child_values.append(c_val)

    meta = nums[idx : idx + n_meta]
    idx += n_meta
    total_meta += sum(meta)

    if n_children == 0:
      node_val = sum(meta)
    else:
      node_val = sum(
          child_values[m - 1] for m in meta if 1 <= m <= len(child_values)
      )

    return total_meta, node_val

  return parse_node()

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 8 -> Part 1: {p1} | Part 2: {p2}")