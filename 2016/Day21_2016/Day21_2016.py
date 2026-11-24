from itertools import permutations
from pathlib import Path
import re


def scramble_password(pw: list[str], instructions: list[str]) -> str:
    s = pw[:]
    for line in instructions:
        line = line.strip()
        if not line:
            continue

        m = re.match(r"^swap position (\d+) with position (\d+)$", line)
        if m:
            i, j = int(m.group(1)), int(m.group(2))
            s[i], s[j] = s[j], s[i]
            continue

        m = re.match(r"^swap letter (\w) with letter (\w)$", line)
        if m:
            a, b = m.group(1), m.group(2)
            s = [b if c == a else (a if c == b else c) for c in s]
            continue

        m = re.match(r"^rotate (left|right) (\d+) steps?$", line)
        if m:
            direction, k = m.group(1), int(m.group(2)) % len(s)
            if k:
                if direction == "left":
                    s = s[k:] + s[:k]
                else:
                    s = s[-k:] + s[:-k]
            continue

        m = re.match(r"^rotate based on position of letter (\w)$", line)
        if m:
            idx = s.index(m.group(1))
            rot = (1 + idx + (1 if idx >= 4 else 0)) % len(s)
            if rot:
                s = s[-rot:] + s[:-rot]
            continue

        m = re.match(r"^reverse positions (\d+) through (\d+)$", line)
        if m:
            i, j = int(m.group(1)), int(m.group(2))
            s[i : j + 1] = reversed(s[i : j + 1])
            continue

        m = re.match(r"^move position (\d+) to position (\d+)$", line)
        if m:
            i, j = int(m.group(1)), int(m.group(2))
            ch = s.pop(i)
            s.insert(j, ch)

    return "".join(s)


def solve(lines: list[str]):
    p1 = scramble_password(list("abcdefgh"), lines)
    target = "fbgdceah"
    p2 = None
    for perm in permutations("abcdefgh"):
        if scramble_password(list(perm), lines) == target:
            p2 = "".join(perm)
            break
    return p1, p2


if __name__ == "__main__":
    lines = Path("input_21.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 21 -> Parte 1: {p1} | Parte 2: {p2}")
