from pathlib import Path


def solve(lines: list[str]):
    ranges = []
    for line in lines:
        s = line.strip()
        if "-" in s:
            a, b = map(int, s.split("-", 1))
            ranges.append((a, b))

    ranges.sort()
    merged = []
    for a, b in ranges:
        if not merged or a > merged[-1][1] + 1:
            merged.append([a, b])
        else:
            if b > merged[-1][1]:
                merged[-1][1] = b

    # Primera IP no bloqueada (empezando en 0)
    p1 = 0
    for a, b in merged:
        if p1 < a:
            break
        if p1 <= b:
            p1 = b + 1

    # Total de IPs permitidas en el rango 0..4294967295 (2^32 - 1)
    max_ip = 4294967295
    blocked = 0
    for a, b in merged:
        if a > max_ip:
            break
        blocked += min(b, max_ip) - a + 1

    p2 = (max_ip + 1) - blocked
    return p1, p2


if __name__ == "__main__":
    lines = Path("input_20.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 20 -> Parte 1: {p1} | Parte 2: {p2}")
