from collections import deque
from pathlib import Path

class ArcadeVM:
    def __init__(self, program: list[int]):
        self.mem: dict[int, int] = {i: v for i, v in enumerate(program)}
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque()
        self.halted = False

    def _read(self, addr: int) -> int:
        return self.mem.get(addr, 0)

    def _write(self, addr: int, val: int) -> None:
        self.mem[addr] = val

    def _mode(self, offset: int, modes: int) -> int:
        return (modes // (10 ** (offset - 1))) % 10

    def _write_addr(self, offset: int, modes: int) -> int:
        mode = self._mode(offset, modes)
        raw = self._read(self.ip + offset)
        if mode == 0:
            return raw
        if mode == 2:
            return self.rel_base + raw
        raise ValueError(f"Invalid write mode {mode}")

    def _param(self, offset: int, modes: int) -> int:
        mode = self._mode(offset, modes)
        raw = self._read(self.ip + offset)
        if mode == 0:
            return self._read(raw)
        if mode == 1:
            return raw
        if mode == 2:
            return self._read(self.rel_base + raw)
        raise ValueError(f"Unknown mode {mode}")

    def step_until_output(self, joystick_provider) -> int | None:
        while True:
            instr = self._read(self.ip)
            op = instr % 100
            modes = instr // 100

            if op == 99:
                self.halted = True
                return None
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
                if not self.inputs:
                    self.inputs.append(joystick_provider())
                self._write(self._write_addr(1, modes), self.inputs.popleft())
                self.ip += 2
            elif op == 4:
                out = self._param(1, modes)
                self.ip += 2
                return out
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

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]

    vm1 = ArcadeVM(program)
    grid1: dict[tuple[int, int], int] = {}
    while not vm1.halted:
        x = vm1.step_until_output(lambda: 0)
        if x is None:
            break
        y = vm1.step_until_output(lambda: 0)
        tile = vm1.step_until_output(lambda: 0)
        if y is None or tile is None:
            break
        grid1[(x, y)] = tile
    p1 = sum(1 for t in grid1.values() if t == 2)

    prog2 = program.copy()
    prog2[0] = 2
    vm2 = ArcadeVM(prog2)
    ball_x = 0
    paddle_x = 0
    score = 0

    def steer() -> int:
        if ball_x < paddle_x:
            return -1
        if ball_x > paddle_x:
            return 1
        return 0

    while not vm2.halted:
        x = vm2.step_until_output(steer)
        if x is None:
            break
        y = vm2.step_until_output(steer)
        val = vm2.step_until_output(steer)
        if y is None or val is None:
            break
        if x == -1 and y == 0:
            score = val
        else:
            if val == 3:
                paddle_x = x
            elif val == 4:
                ball_x = x

    return p1, score

if __name__ == "__main__":
    raw = Path("Day13_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 13 - Part 1: {p1}, Part 2: {p2}")