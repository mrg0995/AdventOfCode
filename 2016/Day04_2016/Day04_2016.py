import hashlib
from pathlib import Path
import re


def solve(lines: list[str]):
    pat = re.compile(r"^([a-z-]+)-(\d+)\[([a-z]{5})\]$")
    real_sum = 0
    northpole_sector = None

    for line in lines:
        m = pat.match(line.strip())
        if not m:
            continue
        enc_name, sector_s, checksum = m.groups()
        sector = int(sector_s)

        letters = enc_name.replace("-", "")
        counts = {}
        for ch in letters:
            counts[ch] = counts.get(ch, 0) + 1
        expected = "".join(
            ch for ch, _ in sorted(counts.items(), key=lambda kv: (-kv[1], kv[0]))[:5]
        )

        if expected == checksum:
            real_sum += sector
            shift = sector % 26
            decrypted = []
            for ch in enc_name:
                if ch == "-":
                    decrypted.append(" ")
                else:
                    idx = (ord(ch) - ord("a") + shift) % 26
                    decrypted.append(chr(ord("a") + idx))
            if "northpole" in "".join(decrypted):
                northpole_sector = sector

    return real_sum, northpole_sector


if __name__ == "__main__":
    lines = Path("input_04.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 04 -> Parte 1: {p1} | Parte 2: {p2}")
