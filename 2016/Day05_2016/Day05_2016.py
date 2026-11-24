import hashlib
from pathlib import Path


def solve(door_id: str):
    door_id = door_id.strip()
    pass1 = []
    pass2 = [None] * 8
    found1 = False
    found2 = 0
    idx = 0

    while not (found1 and found2 == 8):
        h = hashlib.md5(f"{door_id}{idx}".encode()).hexdigest()
        if h.startswith("00000"):
            if not found1:
                pass1.append(h[5])
                if len(pass1) == 8:
                    found1 = True
            pos_ch = h[5]
            if pos_ch in "01234567":
                pos = int(pos_ch)
                if pass2[pos] is None:
                    pass2[pos] = h[6]
                    found2 += 1
        idx += 1

    return "".join(pass1), "".join(pass2)


if __name__ == "__main__":
    raw = Path("input_05.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 05 -> Parte 1: {p1} | Parte 2: {p2}")
