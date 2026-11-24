from pathlib import Path
import sys

def knot_round(
    values: list[int], lengths: list[int], pos: int = 0, skip: int = 0
) -> tuple[int, int]:
    n = len(values)
    for length in lengths:
        for i in range(length // 2):
            a = (pos + i) % n
            b = (pos + length - 1 - i) % n
            values[a], values[b] = values[b], values[a]
        pos = (pos + length + skip) % n
        skip += 1
    return pos, skip

def solve_part1(raw_input: str) -> int:
    lengths = [int(x.strip()) for x in raw_input.strip().split(",") if x.strip()]
    values = list(range(256))
    knot_round(values, lengths)
    return values[0] * values[1]

def knot_hash(raw_input: str) -> str:
    lengths = [ord(ch) for ch in raw_input.strip()] + [17, 31, 73, 47, 23]
    values = list(range(256))
    pos = 0
    skip = 0

    for _ in range(64):
        pos, skip = knot_round(values, lengths, pos, skip)

    dense: list[int] = []
    for block in range(16):
        xor_val = 0
        for i in range(16):
            xor_val ^= values[block * 16 + i]
        dense.append(xor_val)

    return "".join(f"{b:02x}" for b in dense)

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input10.txt")
    raw = path.read_text(encoding="utf-8").strip() if path.exists() else ""
    print(f"Día 10 - Parte 1: {solve_part1(raw)}")
    print(f"Día 10 - Parte 2: {knot_hash(raw)}")

if __name__ == "__main__":
    main()