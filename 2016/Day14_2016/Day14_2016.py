import hashlib
from pathlib import Path
import re


def solve(salt: str):
    salt = salt.strip()
    cache = {}

    def stretched_hash(idx: int, stretches: int) -> str:
        key = (idx, stretches)
        if key in cache:
            return cache[key]
        h = hashlib.md5(f"{salt}{idx}".encode()).hexdigest()
        for _ in range(stretches):
            h = hashlib.md5(h.encode()).hexdigest()
        cache[key] = h
        return h

    def find_64th(stretches: int) -> int:
        keys = []
        idx = 0
        while len(keys) < 64:
            h = stretched_hash(idx, stretches)
            m3 = re.search(r"(.)\1\1", h)
            if m3:
                ch = m3.group(1)
                five_seq = ch * 5
                for j in range(idx + 1, idx + 1001):
                    if five_seq in stretched_hash(j, stretches):
                        keys.append(idx)
                        break
            idx += 1
        return keys[63]

    p1 = find_64th(0)
    cache.clear()
    p2 = find_64th(2016)
    return p1, p2


if __name__ == "__main__":
    raw = Path("input_14.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 14 -> Parte 1: {p1} | Parte 2: {p2}")
