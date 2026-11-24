from pathlib import Path
import sys

def parse_moves(raw: str) -> list[tuple]:
    moves: list[tuple] = []
    for token in raw.strip().split(","):
        token = token.strip()
        if not token:
            continue
        kind = token[0]
        payload = token[1:]
        if kind == "s":
            moves.append(("s", int(payload)))
        elif kind == "x":
            a_str, b_str = payload.split("/", 1)
            moves.append(("x", int(a_str), int(b_str)))
        elif kind == "p":
            a_ch, b_ch = payload.split("/", 1)
            moves.append(("p", a_ch, b_ch))
        else:
            raise ValueError(f"Movimiento desconocido: {token}")
    return moves

def apply_dance(line: list[str], moves: list[tuple]) -> None:
    n = len(line)
    for move in moves:
        kind = move[0]
        if kind == "s":
            k = move[1] % n
            if k:
                line[:] = line[-k:] + line[:-k]
        elif kind == "x":
            _, i, j = move
            line[i], line[j] = line[j], line[i]
        elif kind == "p":
            _, ch_a, ch_b = move
            ia = line.index(ch_a)
            ib = line.index(ch_b)
            line[ia], line[ib] = line[ib], line[ia]

def dance_times(moves: list[tuple], iterations: int) -> str:
    line = list("abcdefghijklmnop")
    seen: dict[str, int] = {}
    history: list[str] = []

    for step in range(iterations):
        state = "".join(line)
        if state in seen:
            cycle_start = seen[state]
            cycle_len = step - cycle_start
            offset = (iterations - cycle_start) % cycle_len
            return history[cycle_start + offset]
        seen[state] = step
        history.append(state)
        apply_dance(line, moves)

    return "".join(line)

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input16.txt")
    raw = path.read_text(encoding="utf-8").strip() if path.exists() else "s1,x3/4,pe/b"
    moves = parse_moves(raw)
    print(f"Día 16 - Parte 1: {dance_times(moves, 1)}")
    print(f"Día 16 - Parte 2: {dance_times(moves, 1_000_000_000)}")

if __name__ == "__main__":
    main()