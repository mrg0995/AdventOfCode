import re
from pathlib import Path

def solve(lines: list[str]):
    ing = []
    pat = re.compile(r"capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)")
    for line in lines:
        m = pat.search(line)
        if m:
            ing.append(tuple(map(int, m.groups())))

    best_any = 0
    best_500 = 0

    def backtrack(idx, rem, totals):
        nonlocal best_any, best_500
        if idx == len(ing) - 1:
            new_t = [totals[j] + rem * ing[idx][j] for j in range(5)]
            if all(new_t[j] > 0 for j in range(4)):
                score = new_t[0] * new_t[1] * new_t[2] * new_t[3]
                if score > best_any:
                    best_any = score
                if new_t[4] == 500 and score > best_500:
                    best_500 = score
            return

        for amt in range(rem + 1):
            nxt = [totals[j] + amt * ing[idx][j] for j in range(5)]
            backtrack(idx + 1, rem - amt, nxt)

    backtrack(0, 100, [0, 0, 0, 0, 0])
    return best_any, best_500

if __name__ == "__main__":
    lines = Path("input_15.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 15 -> Parte 1: {p1} | Parte 2: {p2}")
