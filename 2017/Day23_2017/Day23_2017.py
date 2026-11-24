import math
from pathlib import Path
import sys

def get_val(tok: str, regs: dict[str, int]) -> int:
    try:
        return int(tok)
    except ValueError:
        return regs.get(tok, 0)

def run_part1(insts: list[list[str]]) -> int:
    regs: dict[str, int] = {}
    ip = 0
    muls = 0

    while 0 <= ip < len(insts):
        op, x, y = insts[ip]
        if op == "set":
            regs[x] = get_val(y, regs)
            ip += 1
        elif op == "sub":
            regs[x] = regs.get(x, 0) - get_val(y, regs)
            ip += 1
        elif op == "mul":
            regs[x] = regs.get(x, 0) * get_val(y, regs)
            muls += 1
            ip += 1
        elif op == "jnz":
            if get_val(x, regs) != 0:
                ip += get_val(y, regs)
            else:
                ip += 1

    return muls

def is_prime(n: int) -> bool:
    if n < 2:
        return False
    if n % 2 == 0:
        return n == 2
    limit = math.isqrt(n)
    d = 3
    while d <= limit:
        if n % d == 0:
            return False
        d += 2
    return True

def solve_part2(insts: list[list[str]]) -> int:
    if not insts:
        return 0
    raw_b = int(insts[0][2])
    start = raw_b * 100 + 100_000
    end = start + 17_000
    return sum(1 for b in range(start, end + 1, 17) if not is_prime(b))

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input23.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    insts = [line.split() for line in lines if line.strip()]
    print(f"Día 23 - Parte 1: {run_part1(insts)}")
    print(f"Día 23 - Parte 2: {solve_part2(insts)}")

if __name__ == "__main__":
    main()