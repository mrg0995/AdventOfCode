from collections import deque
from pathlib import Path

class NicVM:
    def __init__(self, program: list[int], addr: int):
        self.mem: dict[int, int] = {i: v for i, v in enumerate(program)}
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque([addr])
        self.out_buf: list[int] = []
        self.idle_reads = 0

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

    def step_once(self) -> tuple[int, int, int] | None:
        instr = self._read(self.ip)
        op = instr % 100
        modes = instr // 100

        if op == 99:
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
            if self.inputs:
                val = self.inputs.popleft()
                self.idle_reads = 0
            else:
                val = -1
                self.idle_reads += 1
            self._write(self._write_addr(1, modes), val)
            self.ip += 2
        elif op == 4:
            out_val = self._param(1, modes)
            self.ip += 2
            self.out_buf.append(out_val)
            if len(self.out_buf) == 3:
                pkt = (self.out_buf[0], self.out_buf[1], self.out_buf[2])
                self.out_buf.clear()
                return pkt
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

        return None

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    nics = [NicVM(program, i) for i in range(50)]

    p1: int | None = None
    nat_pkt: tuple[int, int] | None = None
    last_nat_y: int | None = None
    p2: int | None = None

    while p1 is None or p2 is None:
        progressed = False
        for nic in nics:
            pkt = nic.step_once()
            if pkt is not None:
                progressed = True
                dest, x, y = pkt
                if dest == 255:
                    if p1 is None:
                        p1 = y
                    nat_pkt = (x, y)
                elif 0 <= dest < 50:
                    nics[dest].inputs.extend((x, y))
                    nics[dest].idle_reads = 0
            elif nic.inputs or nic.idle_reads == 0:
                progressed = True

        if not progressed and all(not n.inputs and n.idle_reads >= 2 for n in nics):
            if nat_pkt is not None:
                nx, ny = nat_pkt
                if last_nat_y is not None and ny == last_nat_y and p2 is None:
                    p2 = ny
                    break
                last_nat_y = ny
                nics[0].inputs.extend((nx, ny))
                nics[0].idle_reads = 0

    assert p1 is not None and p2 is not None
    return p1, p2

if __name__ == "__main__":
    raw = Path("Day23_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 23 - Part 1: {p1}, Part 2: {p2}")