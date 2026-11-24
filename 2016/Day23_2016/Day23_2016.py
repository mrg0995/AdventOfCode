from pathlib import Path


def solve(lines: list[str]):
    base_prog = [line.strip().split() for line in lines if line.strip()]

    def run(init_a: int, optimize_mult: bool = False):
        prog = [row[:] for row in base_prog]
        regs = {"a": init_a, "b": 0, "c": 0, "d": 0}
        ip = 0

        def val(x):
            return regs[x] if x in regs else int(x)

        while 0 <= ip < len(prog):
            # Optimización de bucle de multiplicación típico en AoC 2016 Día 23:
            # cpy b c; inc a; dec c; jnz c -2; dec d; jnz d -5
            if (
                optimize_mult
                and ip + 5 < len(prog)
                and prog[ip][0] == "cpy"
                and prog[ip + 1][0] == "inc"
                and prog[ip + 2][0] == "dec"
                and prog[ip + 3][0] == "jnz"
                and prog[ip + 4][0] == "dec"
                and prog[ip + 5][0] == "jnz"
            ):
                acc = prog[ip + 1][1]
                loop_var = prog[ip + 2][1]
                outer_var = prog[ip + 4][1]
                if (
                    prog[ip][2] == loop_var
                    and prog[ip + 3][1] == loop_var
                    and prog[ip + 3][2] == "-2"
                    and prog[ip + 5][1] == outer_var
                    and prog[ip + 5][2] == "-5"
                    and acc in regs
                    and loop_var in regs
                    and outer_var in regs
                ):
                    regs[acc] += abs(regs[loop_var]) * abs(regs[outer_var])
                    regs[loop_var] = 0
                    regs[outer_var] = 0
                    ip += 6
                    continue

            parts = prog[ip]
            op = parts[0]

            if op == "cpy":
                if parts[2] in regs:
                    regs[parts[2]] = val(parts[1])
                ip += 1
            elif op == "inc":
                if parts[1] in regs:
                    regs[parts[1]] += 1
                ip += 1
            elif op == "dec":
                if parts[1] in regs:
                    regs[parts[1]] -= 1
                ip += 1
            elif op == "jnz":
                if val(parts[1]) != 0:
                    ip += val(parts[2])
                else:
                    ip += 1
            elif op == "tgl":
                target_ip = ip + val(parts[1])
                if 0 <= target_ip < len(prog):
                    t_parts = prog[target_ip]
                    if len(t_parts) == 2:
                        t_parts[0] = "dec" if t_parts[0] == "inc" else "inc"
                    elif len(t_parts) == 3:
                        t_parts[0] = "cpy" if t_parts[0] == "jnz" else "jnz"
                ip += 1

        return regs["a"]

    return run(7, False), run(12, True)


if __name__ == "__main__":
    lines = Path("input_23.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 23 -> Parte 1: {p1} | Parte 2: {p2}")
