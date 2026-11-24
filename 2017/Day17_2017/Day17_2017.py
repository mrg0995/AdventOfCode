def solve_part1(steps: int, insertions: int = 2017) -> int:
    buf = [0]
    pos = 0
    for val in range(1, insertions + 1):
        pos = ((pos + steps) % len(buf)) + 1
        buf.insert(pos, val)
    return buf[(pos + 1) % len(buf)]

def solve_part2(steps: int, insertions: int = 50_000_000) -> int:
    pos = 0
    after_zero = 0
    for val in range(1, insertions + 1):
        pos = ((pos + steps) % val) + 1
        if pos == 1:
            after_zero = val
    return after_zero

def main() -> None:
    steps = 371
    print(f"Día 17 - Parte 1: {solve_part1(steps)}")
    print(f"Día 17 - Parte 2: {solve_part2(steps)}")

if __name__ == "__main__":
    main()