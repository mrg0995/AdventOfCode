from pathlib import Path

def run_intcode(program: list[int], noun: int, verb: int) -> int:
    mem = program.copy()
    mem[1] = noun
    mem[2] = verb
    ip = 0

    while ip < len(mem):
        op = mem[ip]
        if op == 99:
            break
        a, b, dest = mem[ip + 1], mem[ip + 2], mem[ip + 3]
        if op == 1:
            mem[dest] = mem[a] + mem[b]
        elif op == 2:
            mem[dest] = mem[a] * mem[b]
        else:
            raise ValueError(f"Unknown opcode {op} at {ip}")
        ip += 4

    return mem[0]

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    p1 = run_intcode(program, 12, 2)

    target = 19690720
    p2 = -1
    for noun in range(100):
        for verb in range(100):
            if run_intcode(program, noun, verb) == target:
                p2 = 100 * noun + verb
                break
        if p2 != -1:
            break

    return p1, p2

if __name__ == "__main__":
    raw = Path("Day02_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 2 - Part 1: {p1}, Part 2: {p2}")