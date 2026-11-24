from pathlib import Path
import operator
import sys

OPS = {
    "==": operator.eq,
    "!=": operator.ne,
    "<": operator.lt,
    "<=": operator.le,
    ">": operator.gt,
    ">=": operator.ge,
}

def run_registers(lines: list[str]) -> tuple[int, int]:
    regs: dict[str, int] = {}
    ever_max = 0

    for line in lines:
        line = line.strip()
        if not line:
            continue
        parts = line.split()
        target_reg, action, amount_str, _, cond_reg, op_str, cond_val_str = parts
        amount = int(amount_str)
        cond_val = int(cond_val_str)

        cond_current = regs.get(cond_reg, 0)
        if OPS[op_str](cond_current, cond_val):
            delta = amount if action == "inc" else -amount
            new_val = regs.get(target_reg, 0) + delta
            regs[target_reg] = new_val
            if new_val > ever_max:
                ever_max = new_val

    final_max = max(regs.values()) if regs else 0
    return final_max, ever_max

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input08.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    part1, part2 = run_registers(lines)
    print(f"Día 8 - Parte 1: {part1}")
    print(f"Día 8 - Parte 2: {part2}")

if __name__ == "__main__":
    main()