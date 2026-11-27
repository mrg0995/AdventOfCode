from collections import deque
from pathlib import Path

class IntcodeVM:
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

    def run_until_output_or_halt(self) -> int | None:
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
                    return None
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

def run_robot(program: list[int], start_color: int) -> dict[tuple[int, int], int]:
    vm = IntcodeVM(program)
    dirs = [(0, -1), (1, 0), (0, 1), (-1, 0)]
    d_idx = 0
    x = y = 0
    grid: dict[tuple[int, int], int] = {(0, 0): start_color}

    while not vm.halted:
        vm.inputs.append(grid.get((x, y), 0))
        color = vm.run_until_output_or_halt()
        if vm.halted or color is None:
            break
        turn = vm.run_until_output_or_halt()
        if turn is None:
            break

        grid[(x, y)] = color
        d_idx = (d_idx + (1 if turn == 1 else -1)) % 4
        dx, dy = dirs[d_idx]
        x += dx
        y += dy

    return grid

def render_grid(grid: dict[tuple[int, int], int]) -> str:
    whites = [pos for pos, col in grid.items() if col == 1]
    if not whites:
        return ""
    min_x = min(p[0] for p in whites)
    max_x = max(p[0] for p in whites)
    min_y = min(p[1] for p in whites)
    max_y = max(p[1] for p in whites)

    rows = []
    for y in range(min_y, max_y + 1):
        rows.append("".join("#" if grid.get((x, y), 0) == 1 else " " for x in range(min_x, max_x + 1)))
    return "\n".join(rows)

def solve(raw: str) -> tuple[int, str]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    p1 = len(run_robot(program, 0))
    p2 = render_grid(run_robot(program, 1))
    return p1, p2

if __name__ == "__main__":
    raw = Path("Day11_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 11 - Part 1: {p1}\nDay 11 - Part 2:\n{p2}")