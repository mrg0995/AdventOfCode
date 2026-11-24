from pathlib import Path


def solve(data: str, rows_p1: int = 40, rows_p2: int = 400000):
    row = data.strip()

    def next_row(r: str) -> str:
        padded = f".{r}."
        out = []
        for i in range(1, len(padded) - 1):
            left, center, right = padded[i - 1], padded[i], padded[i + 1]
            trap = (
                (left == "^" and center == "^" and right == ".")
                or (center == "^" and right == "^" and left == ".")
                or (left == "^" and center == "." and right == ".")
                or (right == "^" and center == "." and left == ".")
            )
            out.append("^" if trap else ".")
        return "".join(out)

    cur = row
    safe_p1 = cur.count(".")
    safe_p2 = cur.count(".")

    for r_idx in range(2, rows_p2 + 1):
        cur = next_row(cur)
        s_cnt = cur.count(".")
        safe_p2 += s_cnt
        if r_idx <= rows_p1:
            safe_p1 += s_cnt

    return safe_p1, safe_p2


if __name__ == "__main__":
    raw = Path("input_18.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 18 -> Parte 1: {p1} | Parte 2: {p2}")
