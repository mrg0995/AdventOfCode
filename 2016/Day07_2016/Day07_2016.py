from pathlib import Path
import re


def has_abba(s: str) -> bool:
    for i in range(len(s) - 3):
        if s[i] != s[i + 1] and s[i] == s[i + 3] and s[i + 1] == s[i + 2]:
            return True
    return False


def get_abas(s: str) -> list[str]:
    abas = []
    for i in range(len(s) - 2):
        if s[i] != s[i + 1] and s[i] == s[i + 2]:
            abas.append(s[i : i + 3])
    return abas


def solve(lines: list[str]):
    tls_count = 0
    ssl_count = 0

    for line in lines:
        s = line.strip()
        if not s:
            continue
        hyper = re.findall(r"\[([^]]+)\]", s)
        supernet = re.split(r"\[[^]]+\]", s)

        if any(has_abba(seg) for seg in supernet) and not any(has_abba(seg) for seg in hyper):
            tls_count += 1

        abas = []
        for seg in supernet:
            abas.extend(get_abas(seg))
        hyper_joined = " ".join(hyper)
        if any(f"{aba[1]}{aba[0]}{aba[1]}" in hyper_joined for aba in abas):
            ssl_count += 1

    return tls_count, ssl_count


if __name__ == "__main__":
    lines = Path("input_07.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 07 -> Parte 1: {p1} | Parte 2: {p2}")
