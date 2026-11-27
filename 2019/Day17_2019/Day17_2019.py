from collections import deque
from pathlib import Path

class AsciiVM:
    def __init__(self, program: list[int], inputs: list[int] | None = None):
        self.mem: dict[int, int] = {i: v for i, v in enumerate(program)}
        self.ip = 0
        self.rel_base = 0
        self.inputs: deque[int] = deque(inputs or [])
        self.outputs: list[int] = []

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

def build_full_path(grid: dict[tuple[int, int], str], start: tuple[int, int], init_dir: str) -> list[str]:
    dirs = {"^": (0, -1), ">": (1, 0), "v": (0, 1), "<": (-1, 0)}
    order = ["^", ">", "v", "<"]
    d_idx = order.index(init_dir)
    x, y = start
    tokens: list[str] = []

    while True:
        dx, dy = dirs[order[d_idx]]
        if grid.get((x + dx, y + dy)) == "#":
            steps = 0
            while grid.get((x + dx, y + dy)) == "#":
                x += dx
                y += dy
                steps += 1
            tokens.append(str(steps))
            continue

        left_idx = (d_idx - 1) % 4
        right_idx = (d_idx + 1) % 4
        ldx, ldy = dirs[order[left_idx]]
        rdx, rdy = dirs[order[right_idx]]

        if grid.get((x + ldx, y + ldy)) == "#":
            tokens.append("L")
            d_idx = left_idx
        elif grid.get((x + rdx, y + rdy)) == "#":
            tokens.append("R")
            d_idx = right_idx
        else:
            break

    return tokens

def compress_movement(tokens: list[str]) -> tuple[str, str, str, str] | None:
    moves = [(tokens[i], tokens[i + 1]) for i in range(0, len(tokens), 2)]

    def to_str(seq: list[tuple[str, str]]) -> str:
        parts: list[str] = []
        for d, s in seq:
            parts.append(d)
            parts.append(s)
        return ",".join(parts)

    def encode(seq: list[tuple[str, str]], a_s: str, b_s: str, c_s: str) -> str | None:
        res: list[str] = []
        i = 0
        while i < len(seq):
            matched = False
            for name, pat_s in (("A", a_s), ("B", b_s), ("C", c_s)):
                if not pat_s:
                    continue
                pat_len = len(pat_s.split(",")) // 2
                if i + pat_len <= len(seq) and to_str(seq[i : i + pat_len]) == pat_s:
                    res.append(name)
                    i += pat_len
                    matched = True
                    break
            if not matched:
                return None
        main_s = ",".join(res)
        return main_s if len(main_s) <= 20 else None

    for a_len in range(1, min(6, len(moves) + 1)):
        a_seq = moves[:a_len]
        a_s = to_str(a_seq)
        if len(a_s) > 20:
            break

        idx_after_a = 0
        while idx_after_a + a_len <= len(moves) and to_str(moves[idx_after_a : idx_after_a + a_len]) == a_s:
            idx_after_a += a_len

        for b_len in range(1, min(6, len(moves) - idx_after_a + 1)):
            b_seq = moves[idx_after_a : idx_after_a + b_len]
            b_s = to_str(b_seq)
            if len(b_s) > 20:
                break

            i = 0
            while i < len(moves):
                if i + a_len <= len(moves) and to_str(moves[i : i + a_len]) == a_s:
                    i += a_len
                elif i + b_len <= len(moves) and to_str(moves[i : i + b_len]) == b_s:
                    i += b_len
                else:
                    break
            if i >= len(moves):
                main_s = encode(moves, a_s, b_s, "")
                if main_s:
                    return main_s, a_s, b_s, ""
                continue

            for c_len in range(1, min(6, len(moves) - i + 1)):
                c_seq = moves[i : i + c_len]
                c_s = to_str(c_seq)
                if len(c_s) > 20:
                    break
                main_s = encode(moves, a_s, b_s, c_s)
                if main_s:
                    return main_s, a_s, b_s, c_s

    return None

def solve(raw: str) -> tuple[int, int]:
    program = [int(x.strip()) for x in raw.split(",") if x.strip()]
    out1 = AsciiVM(program).run()
    text = "".join(chr(c) for c in out1 if c < 128)

    grid: dict[tuple[int, int], str] = {}
    robot_pos = (0, 0)
    robot_dir = "^"
    for y, line in enumerate(text.splitlines()):
        for x, ch in enumerate(line):
            grid[(x, y)] = ch
            if ch in ("^", ">", "v", "<"):
                robot_pos = (x, y)
                robot_dir = ch

    alignment_sum = 0
    for (x, y), ch in grid.items():
        if ch == "#" and all(
            grid.get((x + dx, y + dy)) == "#"
            for dx, dy in ((0, -1), (0, 1), (-1, 0), (1, 0))
        ):
            alignment_sum += x * y

    tokens = build_full_path(grid, robot_pos, robot_dir)
    compressed = compress_movement(tokens)
    assert compressed is not None, "Could not compress movement routine"
    main_s, a_s, b_s, c_s = compressed

    prog2 = program.copy()
    prog2[0] = 2
    script = f"{main_s}\n{a_s}\n{b_s}\n{c_s}\nn\n"
    ascii_inputs = [ord(ch) for ch in script]
    out2 = AsciiVM(prog2, ascii_inputs).run()
    dust = out2[-1]

    return alignment_sum, dust

if __name__ == "__main__":
    raw = Path("Day17_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 17 - Part 1: {p1}, Part 2: {p2}")