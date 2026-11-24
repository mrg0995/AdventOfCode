from pathlib import Path


def checksum(s: str) -> str:
    while len(s) % 2 == 0:
        s = "".join("1" if s[i] == s[i + 1] else "0" for i in range(0, len(s), 2))
    return s


def fill_disk(init_state: str, disk_len: int) -> str:
    state = init_state
    while len(state) < disk_len:
        b = "".join("0" if c == "1" else "1" for c in reversed(state))
        state = f"{state}0{b}"
    return checksum(state[:disk_len])


def solve(init_str: str):
    s = init_str.strip()
    return fill_disk(s, 272), fill_disk(s, 35651584)


if __name__ == "__main__":
    raw = Path("input_16.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 16 -> Parte 1: {p1} | Parte 2: {p2}")
