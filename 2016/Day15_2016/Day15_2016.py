from pathlib import Path
import re


def solve(lines: list[str]):
    discs = []
    pat = re.compile(r"Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+)\.")
    for line in lines:
        m = pat.search(line.strip())
        if m:
            idx, pos, start = int(m.group(1)), int(m.group(2)), int(m.group(3))
            discs.append((idx, pos, start))

    def first_time(extra_disc):
        all_d = discs + ([extra_disc] if extra_disc else [])
        t = 0
        step = 1
        for idx, pos, start in all_d:
            while (start + t + idx) % pos != 0:
                t += step
            # Como los tamaños de disco en AoC 2016 suelen ser coprimos, mcm/step avanza limpiamente
            from math import gcd
            step = step * pos // gcd(step, pos)
        return t

    p1 = first_time(None)
    p2 = first_time((len(discs) + 1, 11, 0))
    return p1, p2


if __name__ == "__main__":
    lines = Path("input_15.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 15 -> Parte 1: {p1} | Parte 2: {p2}")
