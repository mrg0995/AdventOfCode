from collections import deque
from pathlib import Path
import sys

def val_of(token: str, regs: dict[str, int]) -> int:
    try:
        return int(token)
    except ValueError:
        return regs.get(token, 0)

def run_part1(insts: list[list[str]]) -> int:
    regs: dict[str, int] = {}
    ip = 0
    last_sound = 0

    while 0 <= ip < len(insts):
        op, *args = insts[ip]
        if op == "snd":
            last_sound = val_of(args[0], regs)
            ip += 1
        elif op == "set":
            regs[args[0]] = val_of(args[1], regs)
            ip += 1
        elif op == "add":
            regs[args[0]] = regs.get(args[0], 0) + val_of(args[1], regs)
            ip += 1
        elif op == "mul":
            regs[args[0]] = regs.get(args[0], 0) * val_of(args[1], regs)
            ip += 1
        elif op == "mod":
            regs[args[0]] = regs.get(args[0], 0) % val_of(args[1], regs)
            ip += 1
        elif op == "rcv":
            if val_of(args[0], regs) != 0:
                return last_sound
            ip += 1
        elif op == "jgz":
            if val_of(args[0], regs) > 0:
                ip += val_of(args[1], regs)
            else:
                ip += 1

    return last_sound

def run_part2(insts: list[list[str]]) -> int:
    regs = [{"p": 0}, {"p": 1}]
    ips = [0, 0]
    queues = [deque(), deque()]
    sends = [0, 0]

    def step(pid: int) -> bool:
        ip = ips[pid]
        if not (0 <= ip < len(insts)):
            return False
        op, *args = insts[ip]
        r = regs[pid]

        if op == "snd":
            queues[1 - pid].append(val_of(args[0], r))
            sends[pid] += 1
            ips[pid] = ip + 1
            return True
        if op == "set":
            r[args[0]] = val_of(args[1], r)
            ips[pid] = ip + 1
            return True
        if op == "add":
            r[args[0]] = r.get(args[0], 0) + val_of(args[1], r)
            ips[pid] = ip + 1
            return True
        if op == "mul":
            r[args[0]] = r.get(args[0], 0) * val_of(args[1], r)
            ips[pid] = ip + 1
            return True
        if op == "mod":
            r[args[0]] = r.get(args[0], 0) % val_of(args[1], r)
            ips[pid] = ip + 1
            return True
        if op == "rcv":
            if not queues[pid]:
                return False
            r[args[0]] = queues[pid].popleft()
            ips[pid] = ip + 1
            return True
        if op == "jgz":
            if val_of(args[0], r) > 0:
                ips[pid] = ip + val_of(args[1], r)
            else:
                ips[pid] = ip + 1
            return True
        return False

    while True:
        p0 = step(0)
        p1 = step(1)
        if not p0 and not p1:
            break

    return sends[1]

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input18.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    insts = [line.split() for line in lines if line.strip()]
    print(f"Día 18 - Parte 1: {run_part1(insts)}")
    print(f"Día 18 - Parte 2: {run_part2(insts)}")

if __name__ == "__main__":
    main()