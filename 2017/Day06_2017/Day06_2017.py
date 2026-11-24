from pathlib import Path
import sys

def parse_banks(text: str) -> list[int]:
    return [int(x) for x in text.strip().split() if x]

def reallocate_until_cycle(initial_banks: list[int]) -> tuple[int, int]:
    banks = initial_banks[:]
    n = len(banks)
    seen: dict[tuple[int, ...], int] = {}
    cycles = 0

    while True:
        state = tuple(banks)
        if state in seen:
            loop_size = cycles - seen[state]
            return cycles, loop_size

        seen[state] = cycles

        max_blocks = max(banks)
        idx = banks.index(max_blocks)
        banks[idx] = 0

        for step in range(1, max_blocks + 1):
            banks[(idx + step) % n] += 1

        cycles += 1

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input06.txt")
    raw = path.read_text(encoding="utf-8") if path.exists() else "0 2 7 0"
    part1, part2 = reallocate_until_cycle(parse_banks(raw))
    print(f"Día 6 - Parte 1: {part1}")
    print(f"Día 6 - Parte 2: {part2}")

if __name__ == "__main__":
    main()