from pathlib import Path

def solve(lines: list[str]):
    code_len = 0
    mem_len = 0
    enc_len = 0
    for line in lines:
        s = line.strip()
        if not s:
            continue
        code_len += len(s)
        mem_len += len(bytes(s[1:-1], "utf-8").decode("unicode_escape"))
        enc_len += len(s) + s.count("\\") + s.count('"') + 2
    return code_len - mem_len, enc_len - code_len

if __name__ == "__main__":
    lines = Path("input_08.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 08 -> Parte 1: {p1} | Parte 2: {p2}")
