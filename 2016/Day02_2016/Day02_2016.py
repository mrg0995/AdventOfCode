from pathlib import Path


def solve(lines: list[str]):
    pad1 = [
        ["1", "2", "3"],
        ["4", "5", "6"],
        ["7", "8", "9"],
    ]
    pad2 = [
        [None, None, "1", None, None],
        [None, "2", "3", "4", None],
        ["5", "6", "7", "8", "9"],
        [None, "A", "B", "C", None],
        [None, None, "D", None, None],
    ]
    moves = {"U": (-1, 0), "D": (1, 0), "L": (0, -1), "R": (0, 1)}

    def walk(pad, start_r, start_c):
        r, c = start_r, start_c
        code = []
        for line in lines:
            for ch in line.strip():
                dr, dc = moves.get(ch, (0, 0))
                nr, nc = r + dr, c + dc
                if 0 <= nr < len(pad) and 0 <= nc < len(pad[0]) and pad[nr][nc] is not None:
                    r, c = nr, nc
            if line.strip():
                code.append(pad[r][c])
        return "".join(code)

    return walk(pad1, 1, 1), walk(pad2, 2, 0)


if __name__ == "__main__":
    lines = Path("input_02.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 02 -> Parte 1: {p1} | Parte 2: {p2}")
