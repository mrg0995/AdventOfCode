from collections import defaultdict, deque
from pathlib import Path

class IntcodeVM:
    def __init__(self, program: list[int], inputs: list[int] | None = None):
        self.mem: defaultdict[int, int] = defaultdict(int)
        for i, val in enumerate(program):
            self.mem[i] = val
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque(inputs or [])
        self.outputs: list[int] = []

    def _mode(self, offset: int, modes: int) -> int:
        return (modes // (10 ** (offset - 1))) % 10

    def _read_addr(self, offset: int, modes: int) -> int:
        mode = self._mode(offset, modes)
        raw_val = self.mem[self.ip + offset]
        if mode == 0:
            return raw_val
        if mode == 2:
            return self.rel_base + raw_val
        raise ValueError(f"Write parameter cannot use immediate mode at ip={self.ip}")

    def _get_param(self, offset: int, modes: int) -> int:
        mode = self._mode(offset, modes)
        raw_val = self.mem[self.ip + offset]
        if mode == 0:
            return self.mem[raw_val]
        if mode == 1:
            return raw_val
        if mode == 2:
            return self.mem[self.rel_base + raw_val]
        raise ValueError(f"Unknown mode {mode} at ip={self.ip}")

    def run(self) -> list[int]:
        while True:
            instr = self.mem[self.ip]
            op = instr % 100
            modes = instr // 100

            if op == 99:
                return self.outputs
            elif op in (1, 2, 7, 8):
                a = self._get_param(1, modes)
                b = self._get_param(2, modes)
                dest = self._read_addr(3, modes)
                if op == 1:
                    self.mem[dest] = a + b
                elif op == 2:
                    self.mem[dest] = a * b
                elif op == 7:
                    self.mem[dest] = 1 if a < b else 0
                elif op == 8:
                    self.mem[dest] = 1 if a == b else 0
                self.ip += 4
            elif op == 3:
                dest = self._read_addr(1, modes)
                self.mem[dest] = self.inputs.popleft()
                self.ip += 2
            elif op == 4:
                self.outputs.append(self._get_param(1, modes))
                self.ip += 2
            elif op in (5, 6):
                a = self._get_param(1, modes)
                b = self._get_param(2, modes)
                if (op == 5 and a != 0) or (op == 6 and a == 0):
                    self.ip = b
                else:
                    self.ip += 3
            elif op == 9:
                self.rel_base += self._get_param(1, modes)
                self.ip += 2
            else:
                raise ValueError(f"Unknown opcode {op} at ip={self.ip}")

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    p1 = IntcodeVM(program, [1]).run()[-1]
    p2 = IntcodeVM(program, [2]).run()[-1]
    return p1, p2

if __name__ == "__main__":
    raw = Path("Day09_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 9 - Part 1: {p1}, Part 2: {p2}")