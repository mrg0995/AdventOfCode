from pathlib import Path

def solve(lines: list[str]):
    paper = 0
    ribbon = 0
    for line in lines:
        if not line.strip():
            continue
        l, w, h = sorted(map(int, line.strip().split("x")))
        paper += 2 * (l * w + w * h + h * l) + (l * w)
        ribbon += 2 * (l + w) + (l * w * h)
    return paper, ribbon

if __name__ == "__main__":
    lines = Path("input_02.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 02 -> Parte 1: {p1} | Parte 2: {p2}")
