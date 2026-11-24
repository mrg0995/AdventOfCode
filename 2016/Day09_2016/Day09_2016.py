from pathlib import Path
import re


def decompressed_len(s: str, recursive: bool) -> int:
    total = 0
    i = 0
    n = len(s)
    while i < n:
        if s[i] == "(":
            end = s.index(")", i + 1)
            length, repeat = map(int, s[i + 1 : end].split("x"))
            block = s[end + 1 : end + 1 + length]
            if recursive:
                total += decompressed_len(block, True) * repeat
            else:
                total += len(block) * repeat
            i = end + 1 + length
        else:
            total += 1
            i += 1
    return total


def solve(data: str):
    s = re.sub(r"\s+", "", data)
    return decompressed_len(s, False), decompressed_len(s, True)


if __name__ == "__main__":
    raw = Path("input_09.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 09 -> Parte 1: {p1} | Parte 2: {p2}")
