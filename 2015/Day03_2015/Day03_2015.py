from pathlib import Path

def solve(data: str):
    moves = {"^": (0, 1), "v": (0, -1), ">": (1, 0), "<": (-1, 0)}

    def walk(seq: str):
        x, y = 0, 0
        visited = {(0, 0)}
        for ch in seq:
            dx, dy = moves[ch]
            x, y = x + dx, y + dy
            visited.add((x, y))
        return visited

    seq = data.strip()
    part1 = len(walk(seq))
    part2 = len(walk(seq[0::2]) | walk(seq[1::2]))
    return part1, part2

if __name__ == "__main__":
    raw = Path("input_03.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 03 -> Parte 1: {p1} | Parte 2: {p2}")
