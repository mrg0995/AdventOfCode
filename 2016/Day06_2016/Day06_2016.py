from collections import Counter
from pathlib import Path


def solve(lines: list[str]):
    valid = [line.strip() for line in lines if line.strip()]
    cols = [Counter(row[i] for row in valid) for i in range(len(valid[0]))]
    p1 = "".join(c.most_common(1)[0][0] for c in cols)
    p2 = "".join(c.most_common()[-1][0] for c in cols)
    return p1, p2


if __name__ == "__main__":
    lines = Path("input_06.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 06 -> Parte 1: {p1} | Parte 2: {p2}")
