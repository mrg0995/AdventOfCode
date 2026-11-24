from pathlib import Path

def solve(lines: list[str]):
    insts = []
    for line in lines:
        s = line.strip()
        if not s:
            continue
        op_part, *rest = s.replace(",", "").split()
        insts.append((op_part, rest))

    def run(init_a: int):
        regs = {"a": init_a, "b": 0}
        ip = 0
        while 0 <= ip < len(insts):
            op, args = insts[ip]
            if op == "hlf":
                regs[args[0]] //= 2
                ip += 1
            elif op == "tpl":
                regs[args[0]] *= 3
                ip += 1
            elif op == "inc":
                regs[args[0]] += 1
                ip += 1
            elif op == "jmp":
                ip += int(args[0])
            elif op == "jie":
                if regs[args[0]] % 2 == 0:
                    ip += int(args[1])
                else:
                    ip += 1
            elif op == "jio":
                if regs[args[0]] == 1:
                    ip += int(args[1])
                else:
                    ip += 1
        return regs["b"]

    return run(0), run(1)

if __name__ == "__main__":
    lines = Path("input_23.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 23 -> Parte 1: {p1} | Parte 2: {p2}")
