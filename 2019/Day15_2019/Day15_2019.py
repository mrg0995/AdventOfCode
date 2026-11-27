from collections import deque
from pathlib import Path

class DroidVM:
    def __init__(self, program: list[int]):
        self.mem: dict[int, int] = {i: v for i, v in enumerate(program)}
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque()

    def clone(self) -> "DroidVM":
        copy_vm = DroidVM([])
        copy_vm.mem = self.mem.copy()
        copy_vm.ip = self.ip
        copy_vm.rel_base = self.rel_base
        copy_vm.inputs = deque(self.inputs)
        return copy_vm

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

    def send_move(self, cmd: int) -> int:
        self.inputs.append(cmd)
        while True:
            instr = self._read(self.ip)
            op = instr % 100
            modes = instr // 100

            if op == 99:
                return 0
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
                status = self._param(1, modes)
                self.ip += 2
                return status
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
    moves = {
        1: (0, -1),
        2: (0, 1),
        3: (-1, 0),
        4: (1, 0),
    }

    start_pos = (0, 0)
    visited_states = {start_pos}
    open_tiles: set[tuple[int, int]] = {start_pos}
    oxygen_pos: tuple[int, int] | None = None
    dist_to_oxygen = 0

    q: deque[tuple[tuple[int, int], int, DroidVM]] = deque([(start_pos, 0, DroidVM(program))])

    while q:
        pos, dist, vm = q.popleft()
        for cmd, (dx, dy) in moves.items():
            nxt = (pos[0] + dx, pos[1] + dy)
            if nxt in visited_states:
                continue
            visited_states.add(nxt)

            branch_vm = vm.clone()
            status = branch_vm.send_move(cmd)
            if status == 0:
                continue

            open_tiles.add(nxt)
            if status == 2:
                oxygen_pos = nxt
                dist_to_oxygen = dist + 1
            q.append((nxt, dist + 1, branch_vm))

    assert oxygen_pos is not None

    fill_q: deque[tuple[tuple[int, int], int]] = deque([(oxygen_pos, 0)])
    seen_fill = {oxygen_pos}
    max_minutes = 0

    while fill_q:
        pos, mins = fill_q.popleft()
        if mins > max_minutes:
            max_minutes = mins
        for dx, dy in moves.values():
            nxt = (pos[0] + dx, pos[1] + dy)
            if nxt in open_tiles and nxt not in seen_fill:
                seen_fill.add(nxt)
                fill_q.append((nxt, mins + 1))

    return dist_to_oxygen, max_minutes

if __name__ == "__main__":
    raw = Path("Day15_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 15 - Part 1: {p1}, Part 2: {p2}")