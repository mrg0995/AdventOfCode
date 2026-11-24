from pathlib import Path


def solve(lines: list[str]):
    prog = [line.strip().split() for line in lines if line.strip()]

    def run(init_c: int):
        regs = {"a": 0, "b": 0, "c": init_c, "d": 0}
        ip = 0

        def val(x):
            return regs[x] if x in regs else int(x)

        while 0 <= ip < len(prog):
            parts = prog[ip]
            op = parts[0]
            if op == "cpy":
                if parts[2] in regs:
                    regs[parts[2]] = val(parts[1])
                ip += 1
            elif op == "inc":
                regs[parts[1]] += 1
                ip += 1
            elif op == "dec":
                regs[parts[1]] -= 1
                ip += 1
            elif op == "jnz":
                if val(parts[1]) != 0:
                    ip += val(parts[2])
                else:
                    ip += 1
        return regs["a"]

    return run(0), run(1)


if __name__ == "__main__":
    lines = Path("input_12.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 12 -> Parte 1: {p1} | Parte 2: {p2}")
