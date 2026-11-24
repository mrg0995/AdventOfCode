from collections import deque
import hashlib
from pathlib import Path


def solve(passcode: str):
    passcode = passcode.strip()
    dirs = [("U", 0, -1), ("D", 0, 1), ("L", -1, 0), ("R", 1, 0)]
    open_chars = set("bcdef")

    q = deque([(0, 0, "")])
    shortest = None
    longest = 0

    while q:
        x, y, path = q.popleft()
        if (x, y) == (3, 3):
            if shortest is None:
                shortest = path
            if len(path) > longest:
                longest = len(path)
            continue

        h = hashlib.md5(f"{passcode}{path}".encode()).hexdigest()[:4]
        for i, (ch, dx, dy) in enumerate(dirs):
            nx, ny = x + dx, y + dy
            if h[i] in open_chars and 0 <= nx < 4 and 0 <= ny < 4:
                q.append((nx, ny, path + ch))

    return shortest, longest


if __name__ == "__main__":
    raw = Path("input_17.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 17 -> Parte 1: {p1} | Parte 2: {p2}")
