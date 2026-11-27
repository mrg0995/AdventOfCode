from collections import deque
from pathlib import Path

class SpringVM:
    def __init__(self, program: list[int], inputs: list[int]):
        self.mem: dict[int, int] = {i: v for i, v in enumerate(program)}
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque(inputs)
        self.outputs: list[int] = []

    def _read(self, addr: int) -> int:
        return self.mem.get(addr, 0)

    def _write(self, addr: int, val: int) -> None:
        self.mem[addr] = val

    def _mode(self, offset: int, modes: int) -> int:
        return (modes // (10 ** (offset - 1))) % 10

    def _write_addr(self, offset: int, modes: int) -> int:
        m = self._mode(offset, modes)
        raw = self._read(self.ip + offset)
        if m == 0:
            return raw
        if m == 2:
            return self.rel_base + raw
        raise ValueError(f"Invalid write mode {m}")

    def _param(self, offset: int, modes: int) -> int:
        m = self._mode(offset, modes)
        raw = self._read(self.ip + offset)
        if m == 0:
            return self._read(raw)
        if m == 1:
            return raw
        if m == 2:
            return self._read(self.rel_base + raw)
        raise ValueError(f"Unknown mode {m}")

    def run(self) -> list[int]:
        while True:
            instr = self._read(self.ip)
            op = instr % 100
            modes = instr // 100

            if op == 99:
                return self.outputs
            if op in (1, 2, 7, 8):
                a = self._param(1, modes)
                b = self._param(2, modes)
                dest = self._write_addr(3, modes)
                if op == 1:
                    self._write(dest, a + b)
                elif op == 2:
                    self._write(dest, a * b)
                elif op == 7:
                    self._write(dest, 1 if a < b else 0)
                elif op == 8:
                    self._write(dest, 1 if a == b else 0)
                self.ip += 4
            elif op == 3:
                self._write(self._write_addr(1, modes), self.inputs.popleft())
                self.ip += 2
            elif op == 4:
                self.outputs.append(self._param(1, modes))
                self.ip += 2
            elif op in (5, 6):
                a = self._param(1, modes)
                b = self._param(2, modes)
                if (op == 5 and a != 0) or (op == 6 and a == 0):
                    self.ip = b
                else:
                    self.ip += 3
            elif op == 9:
                self.rel_base += self._param(1, modes)
                self.ip += 2
            else:
                raise ValueError(f"Unknown opcode {op}")

def run_script(program: list[int], script_lines: list[str], cmd: str) -> int:
    full_script = "\n".join(script_lines + [cmd, ""])
    ascii_in = [ord(ch) for ch in full_script]
    out = SpringVM(program, ascii_in).run()
    return out[-1]

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]

    walk_script = [
        "NOT A J",
        "NOT B T",
        "OR T J",
        "NOT C T",
        "OR T J",
        "AND D J",
    ]
    p1 = run_script(program, walk_script, "WALK")

    run_script_lines = [
        "NOT A J",
        "NOT B T",
        "OR T J",
        "NOT C T",
        "OR T J",
        "AND D J",
        "NOT E T",
        "NOT T T",
        "OR H T",
        "AND T J",
    ]
    p2 = run_script(program, run_script_lines, "RUN")

    return p1, p2

if __name__ == "__main__":
    raw = Path("Day21_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 21 - Part 1: {p1}, Part 2: {p2}")