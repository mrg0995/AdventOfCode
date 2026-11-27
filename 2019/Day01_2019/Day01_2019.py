from pathlib import Path

def fuel_part1(mass: int) -> int:
    return (mass // 3) - 2

def fuel_part2(mass: int) -> int:
    total = 0
    f = fuel_part1(mass)
    while f > 0:
        total += f
        f = fuel_part1(f)
    return total

def solve(lines: list[str]) -> tuple[int, int]:
    masses = [int(x.strip()) for x in lines if x.strip()]
    return sum(fuel_part1(m) for m in masses), sum(fuel_part2(m) for m in masses)

if __name__ == "__main__":
    data = Path("Day01_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(data)
    print(f"Day 1 - Part 1: {p1}, Part 2: {p2}")