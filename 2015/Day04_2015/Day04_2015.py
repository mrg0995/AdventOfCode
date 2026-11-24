import hashlib
from pathlib import Path

def solve(secret: str):
    secret = secret.strip()
    n = 1
    p1 = p2 = None
    while p1 is None or p2 is None:
        h = hashlib.md5(f"{secret}{n}".encode()).hexdigest()
        if p1 is None and h.startswith("00000"):
            p1 = n
        if p2 is None and h.startswith("000000"):
            p2 = n
        n += 1
    return p1, p2

if __name__ == "__main__":
    raw = Path("input_04.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 04 -> Parte 1: {p1} | Parte 2: {p2}")
