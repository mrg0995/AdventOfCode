import math
from pathlib import Path

def parse_program(text: str):
    lines = [line.strip() for line in text.strip().splitlines() if line.strip()]
    ip_reg = int(lines[0].split()[1])
    op_map = {
        "addr": 0, "addi": 1, "mulr": 2, "muli": 3,
        "banr": 4, "bani": 5, "borr": 6, "bori": 7,
        "setr": 8, "seti": 9, "gtir": 10, "gtri": 11,
        "gtrr": 12, "eqir": 13, "eqri": 14, "eqrr": 15,
    }
    program = []
    for line in lines[1:]:
        op, a, b, c = line.split()
        program.append((op_map[op], int(a), int(b), int(c)))
    return ip_reg, program

def step_fast(op: int, a: int, b: int, c: int, r: list[int]) -> None:
    if op == 0:
        r[c] = r[a] + r[b]
    elif op == 1:
        r[c] = r[a] + b
    elif op == 2:
        r[c] = r[a] * r[b]
    elif op == 3:
        r[c] = r[a] * b
    elif op == 4:
        r[c] = r[a] & r[b]
    elif op == 5:
        r[c] = r[a] & b
    elif op == 6:
        r[c] = r[a] | r[b]
    elif op == 7:
        r[c] = r[a] | b
    elif op == 8:
        r[c] = r[a]
    elif op == 9:
        r[c] = a
    elif op == 10:
        r[c] = 1 if a > r[b] else 0
    elif op == 11:
        r[c] = 1 if r[a] > b else 0
    elif op == 12:
        r[c] = 1 if r[a] > r[b] else 0
    elif op == 13:
        r[c] = 1 if a == r[b] else 0
    elif op == 14:
        r[c] = 1 if r[a] == b else 0
    elif op == 15:
        r[c] = 1 if r[a] == r[b] else 0

def find_part2_target(ip_reg: int, program: list) -> int:
    r = [1, 0, 0, 0, 0, 0]
    ip = 0
    n_inst = len(program)
    prev_max = 0
    stable_hits = 0

    while 0 <= ip < n_inst:
        r[ip_reg] = ip
        op, a, b, c = program[ip]
        step_fast(op, a, b, c, r)
        ip = r[ip_reg] + 1

        cur_max = max(r)
        if cur_max > 10000:
            if cur_max == prev_max:
                stable_hits += 1
                if stable_hits >= 50:
                    return cur_max
            else:
                prev_max = cur_max
                stable_hits = 0

    return max(r)

def run_vm(ip_reg: int, program: list, r0: int = 0) -> int:
    r = [r0, 0, 0, 0, 0, 0]
    ip = 0
    n_inst = len(program)
    while 0 <= ip < n_inst:
        r[ip_reg] = ip
        op, a, b, c = program[ip]
        step_fast(op, a, b, c, r)
        ip = r[ip_reg] + 1
    return r[0]

def sum_of_divisors(n: int) -> int:
    total = 0
    limit = math.isqrt(n)
    for d in range(1, limit + 1):
        if n % d == 0:
            total += d
            q = n // d
            if q != d:
                total += q
    return total

def solve(raw_input: str) -> tuple[int, int]:
    ip_reg, program = parse_program(raw_input)
    part1 = run_vm(ip_reg, program, r0=0)
    target = find_part2_target(ip_reg, program)
    part2 = sum_of_divisors(target)
    return part1, part2

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 19 -> Part 1: {p1} | Part 2: {p2}")