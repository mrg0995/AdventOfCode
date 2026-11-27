from pathlib import Path

DECK_P1 = 10_007
DECK_P2 = 119_315_717_514_047
SHUFFLES_P2 = 101_741_582_076_661

def track_card_2019(lines: list[str], deck_size: int = DECK_P1) -> int:
    pos = 2019
    for raw in lines:
        line = raw.strip()
        if not line:
            continue
        if line == "deal into new stack":
            pos = (deck_size - 1 - pos) % deck_size
        elif line.startswith("cut "):
            n = int(line.split()[-1])
            pos = (pos - n) % deck_size
        elif line.startswith("deal with increment "):
            n = int(line.split()[-1])
            pos = (pos * n) % deck_size
        else:
            raise ValueError(f"Unknown instruction: {line}")
    return pos

def affine_for_shuffle(lines: list[str], m: int) -> tuple[int, int]:
    a, b = 1, 0
    for raw in lines:
        line = raw.strip()
        if not line:
            continue
        if line == "deal into new stack":
            a = (-a) % m
            b = (-b - 1) % m
        elif line.startswith("cut "):
            n = int(line.split()[-1])
            b = (b - n) % m
        elif line.startswith("deal with increment "):
            n = int(line.split()[-1])
            a = (a * n) % m
            b = (b * n) % m
        else:
            raise ValueError(f"Unknown instruction: {line}")
    return a, b

def poly_pow_mod(a: int, b: int, k: int, m: int) -> tuple[int, int]:
    A, B = 1, 0
    cur_a, cur_b = a % m, b % m
    exp = k
    while exp > 0:
        if exp & 1:
            A = (cur_a * A) % m
            B = (cur_a * B + cur_b) % m
        cur_b = (cur_a * cur_b + cur_b) % m
        cur_a = (cur_a * cur_a) % m
        exp >>= 1
    return A, B

def solve(lines: list[str]) -> tuple[int, int]:
    p1 = track_card_2019(lines, DECK_P1)

    a, b = affine_for_shuffle(lines, DECK_P2)
    A, B = poly_pow_mod(a, b, SHUFFLES_P2, DECK_P2)
    inv_A = pow(A, -1, DECK_P2)
    p2 = ((2020 - B) % DECK_P2) * inv_A % DECK_P2

    return p1, p2

if __name__ == "__main__":
    lines = Path("Day22_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 22 - Part 1: {p1}, Part 2: {p2}")