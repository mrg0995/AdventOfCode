import re
from pathlib import Path

def solve(text: str):
    nums = list(map(int, re.findall(r"\d+", text)))
    row, col = nums[0], nums[1]
    diag = row + col - 1
    index = diag * (diag - 1) // 2 + col

    val = 20151125
    mul = 252533
    mod = 33554393
    factor = pow(mul, index - 1, mod)
    p1 = (val * factor) % mod
    return p1, "Merry Christmas! (50 stars complete)"

if __name__ == "__main__":
    raw = Path("input_25.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 25 -> Parte 1: {p1} | Parte 2: {p2}")
