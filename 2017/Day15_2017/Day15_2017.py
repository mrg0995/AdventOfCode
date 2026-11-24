from pathlib import Path
import re
import sys

FACTOR_A = 16807
FACTOR_B = 48271
MODULUS = 2147483647

def parse_seeds(text: str) -> tuple[int, int]:
    nums = [int(m.group(0)) for m in re.finditer(r"-?\d+", text)]
    if len(nums) < 2:
        return 65, 8921
    return nums[0], nums[1]

def next_val(curr: int, factor: int) -> int:
    return (curr * factor) % MODULUS

def solve_part1(seed_a: int, seed_b: int, pairs: int = 40_000_000) -> int:
    a, b = seed_a, seed_b
    matches = 0
    for _ in range(pairs):
        a = next_val(a, FACTOR_A)
        b = next_val(b, FACTOR_B)
        if (a & 0xFFFF) == (b & 0xFFFF):
            matches += 1
    return matches

def solve_part2(seed_a: int, seed_b: int, pairs: int = 5_000_000) -> int:
    a, b = seed_a, seed_b
    matches = 0
    for _ in range(pairs):
        a = next_val(a, FACTOR_A)
        while (a & 3) != 0:
            a = next_val(a, FACTOR_A)

        b = next_val(b, FACTOR_B)
        while (b & 7) != 0:
            b = next_val(b, FACTOR_B)

        if (a & 0xFFFF) == (b & 0xFFFF):
            matches += 1
    return matches

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input15.txt")
    raw = path.read_text(encoding="utf-8") if path.exists() else ""
    seed_a, seed_b = parse_seeds(raw)
    print(f"Día 15 - Parte 1: {solve_part1(seed_a, seed_b)}")
    print(f"Día 15 - Parte 2: {solve_part2(seed_a, seed_b)}")

if __name__ == "__main__":
    main()