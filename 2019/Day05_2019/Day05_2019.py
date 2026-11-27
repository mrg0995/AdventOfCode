from pathlib import Path

def run_intcode(program: list[int], inputs: list[int]) -> list[int]:
    mem = program.copy()
    ip = 0
    in_idx = 0
    outputs: list[int] = []

    def get_param(offset: int, modes: int) -> int:
        mode = (modes // (10 ** (offset - 1))) % 10
        val = mem[ip + offset]
        return mem[val] if mode == 0 else val

    while ip < len(mem):
        instr = mem[ip]
        op = instr % 100
        modes = instr // 100

        if op == 99:
            break
        elif op in (1, 2, 7, 8):
            a = get_param(1, modes)
            b = get_param(2, modes)
            dest = mem[ip + 3]
            if op == 1:
                mem[dest] = a + b
            elif op == 2:
                mem[dest] = a * b
            elif op == 7:
                mem[dest] = 1 if a < b else 0
            elif op == 8:
                mem[dest] = 1 if a == b else 0
            ip += 4
        elif op == 3:
            dest = mem[ip + 1]
            mem[dest] = inputs[in_idx]
            in_idx += 1
            ip += 2
        elif op == 4:
            outputs.append(get_param(1, modes))
            ip += 2
        elif op in (5, 6):
            a = get_param(1, modes)
            b = get_param(2, modes)
            if (op == 5 and a != 0) or (op == 6 and a == 0):
                ip = b
            else:
                ip += 3
        else:
            raise ValueError(f"Unknown opcode {op} at {ip}")

    return outputs

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    p1 = run_intcode(program, [1])[-1]
    p2 = run_intcode(program, [5])[-1]
    return p1, p2

if __name__ == "__main__":
    raw = Path("Day05_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 5 - Part 1: {p1}, Part 2: {p2}")