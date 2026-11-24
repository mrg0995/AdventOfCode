import re
from pathlib import Path

def solve(lines: list[str]):
    bad_pairs = ("ab", "cd", "pq", "xy")
    p1 = p2 = 0
    for s in lines:
        s = s.strip()
        if not s:
            continue
        vowels = sum(1 for c in s if c in "aeiou")
        double = any(s[i] == s[i + 1] for i in range(len(s) - 1))
        no_bad = not any(bp in s for bp in bad_pairs)
        if vowels >= 3 and double and no_bad:
            p1 += 1

        pair_twice = bool(re.search(r"(..).*\1", s))
        aba = any(s[i] == s[i + 2] for i in range(len(s) - 2))
        if pair_twice and aba:
            p2 += 1
    return p1, p2

if __name__ == "__main__":
    lines = Path("input_05.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 05 -> Parte 1: {p1} | Parte 2: {p2}")
