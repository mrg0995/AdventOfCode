from collections import deque
from pathlib import Path

def beam_at(program: list[int], x: int, y: int) -> int:
    if x < 0 or y < 0:
        return 0
    mem: dict[int, int] = {i: v for i, v in enumerate(program)}
    ip = 0
    rel_base = 0
    inputs = deque([x, y])

    def read(addr: int) -> int:
        return mem.get(addr, 0)

    def write(addr: int, val: int) -> None:
        mem[addr] = val

    def mode(offset: int, modes: int) -> int:
        return (modes // (10 ** (offset - 1))) % 10

    def write_addr(offset: int, modes: int) -> int:
        m = mode(offset, modes)
        raw = read(ip + offset)
        if m == 0:
            return raw
        if m == 2:
            return rel_base + raw
        raise ValueError(f"Invalid write mode {m}")

    def param(offset: int, modes: int) -> int:
        m = mode(offset, modes)
        raw = read(ip + offset)
        if m == 0:
            return read(raw)
        if m == 1:
            return raw
        if m == 2:
            return read(rel_base + raw)
        raise ValueError(f"Unknown mode {m}")

    while True:
        instr = read(ip)
        op = instr % 100
        modes = instr // 100

        if op == 99:
            return 0
        if op in (1, 2, 7, 8):
            a = param(1, modes)
            b = param(2, modes)
            dest = write_addr(3, modes)
            if op == 1:
                write(dest, a + b)
            elif op == 2:
                write(dest, a * b)
            elif op == 7:
                write(dest, 1 if a < b else 0)
            elif op == 8:
                write(dest, 1 if a == b else 0)
            ip += 4
        elif op == 3:
            write(write_addr(1, modes), inputs.popleft())
            ip += 2
        elif op == 4:
            return param(1, modes)
        elif op in (5, 6):
            a = param(1, modes)
            b = param(2, modes)
            if (op == 5 and a != 0) or (op == 6 and a == 0):
                ip = b
            else:
                ip += 3
        elif op == 9:
            rel_base += param(1, modes)
            ip += 2
        else:
            raise ValueError(f"Unknown opcode {op}")

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]

    p1 = sum(
        beam_at(program, x, y)
        for y in range(50)
        for x in range(50)
    )

    x = 0
    y = 100
    size = 100
    while True:
        while beam_at(program, x, y) == 0:
            x += 1
        top_right_x = x + size - 1
        top_right_y = y - (size - 1)
        if top_right_y >= 0 and beam_at(program, top_right_x, top_right_y) == 1:
            p2 = x * 10000 + top_right_y
            break
        y += 1

    return p1, p2

if __name__ == "__main__":
    raw = Path("Day19_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 19 - Part 1: {p1}, Part 2: {p2}")