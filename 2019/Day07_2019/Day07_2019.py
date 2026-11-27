from collections import deque
from itertools import permutations
from pathlib import Path

class IntcodeVM:
    def __init__(self, program: list[int], inputs: list[int] | None = None):
        self.mem = program.copy()
        self.ip = 0
        self.inputs: deque[int] = deque(inputs or [])
        self.outputs: list[int] = []
        self.halted = False

    def _get_param(self, offset: int, modes: int) -> int:
        mode = (modes // (10 ** (offset - 1))) % 10
        val = self.mem[self.ip + offset]
        return self.mem[val] if mode == 0 else val

    def run_until_pause_or_halt(self) -> None:
        while self.ip < len(self.mem):
            instr = self.mem[self.ip]
            op = instr % 100
            modes = instr // 100

            if op == 99:
                self.halted = True
                return
            elif op in (1, 2, 7, 8):
                a = self._get_param(1, modes)
                b = self._get_param(2, modes)
                dest = self.mem[self.ip + 3]
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
                if not self.inputs:
                    return
                dest = self.mem[self.ip + 1]
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
            else:
                raise ValueError(f"Unknown opcode {op} at {self.ip}")

def eval_series(program: list[int], phases: tuple[int, ...]) -> int:
    signal = 0
    for phase in phases:
        vm = IntcodeVM(program, [phase, signal])
        vm.run_until_pause_or_halt()
        signal = vm.outputs[-1]
    return signal

def eval_loop(program: list[int], phases: tuple[int, ...]) -> int:
    amps = [IntcodeVM(program, [p]) for p in phases]
    signal = 0
    idx = 0
    last_e_signal = 0

    while not amps[-1].halted:
        vm = amps[idx]
        vm.inputs.append(signal)
        vm.run_until_pause_or_halt()
        if vm.outputs:
            signal = vm.outputs[-1]
            if idx == len(amps) - 1:
                last_e_signal = signal
        idx = (idx + 1) % len(amps)

    return last_e_signal

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    p1 = max(eval_series(program, perm) for perm in permutations(range(5)))
    p2 = max(eval_loop(program, perm) for perm in permutations(range(5, 10)))
    return p1, p2

if __name__ == "__main__":
    raw = Path("Day07_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 7 - Part 1: {p1}, Part 2: {p2}")