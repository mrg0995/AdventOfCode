from pathlib import Path
import time


def solve(jumps: list[int], is_part2: bool) -> int:
    index = 0
    steps = 0
    length = len(jumps)

    while 0 <= index < length:
        offset = jumps[index]
        jumps[index] += -1 if (is_part2 and offset >= 3) else 1
        index += offset
        steps += 1

    return steps


def main() -> None:
    input_path = Path(r"C:\Users\ProtoMkUser\Desktop\Proyectos\AdventOfCode\2017\Day05_2017\Day05_2017.txt")
    if not input_path.exists():
        print("No se encontró 'input.txt' en el directorio actual.")
        return

    base_jumps = [
        int(line)
        for line in input_path.read_text(encoding="utf-8").splitlines()
        if line.strip()
    ]

    t0 = time.perf_counter()
    part1 = solve(base_jumps.copy(), is_part2=False)
    t1 = (time.perf_counter() - t0) * 1000

    t2 = time.perf_counter()
    part2 = solve(base_jumps.copy(), is_part2=True)
    t3 = (time.perf_counter() - t2) * 1000

    print(f"Parte 1: {part1} pasos ({t1:.1f} ms)")
    print(f"Parte 2: {part2} pasos ({t3:.1f} ms)")


if __name__ == "__main__":
    main()