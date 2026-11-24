import re
from pathlib import Path

def solve(lines: list[str]):
    rules = []
    mol = ""
    for line in lines:
        s = line.strip()
        if " => " in s:
            a, b = s.split(" => ")
            rules.append((a, b))
        elif s:
            mol = s

    distinct = set()
    for src, dst in rules:
        for m in re.finditer(re.escape(src), mol):
            idx = m.start()
            new_m = mol[:idx] + dst + mol[idx + len(src):]
            distinct.add(new_m)

    cur = mol
    steps = 0
    rev_rules = sorted(rules, key=lambda x: len(x[1]), reverse=True)
    while cur != "e":
        progressed = False
        for src, dst in rev_rules:
            if dst in cur:
                cnt = cur.count(dst)
                cur = cur.replace(dst, src)
                steps += cnt
                progressed = True
                break
        if not progressed:
            break

    return len(distinct), steps

if __name__ == "__main__":
    lines = Path("input_19.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 19 -> Parte 1: {p1} | Parte 2: {p2}")
