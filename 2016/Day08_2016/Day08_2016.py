from pathlib import Path
import re


def solve(lines: list[str], rows: int = 6, cols: int = 50):
    screen = [[0] * cols for _ in range(rows)]

    for line in lines:
        s = line.strip()
        if not s:
            continue
        m_rect = re.match(r"^rect (\d+)x(\d+)$", s)
        if m_rect:
            w, h = int(m_rect.group(1)), int(m_rect.group(2))
            for r in range(h):
                for c in range(w):
                    screen[r][c] = 1
            continue

        m_row = re.match(r"^rotate row y=(\d+) by (\d+)$", s)
        if m_row:
            r, by = int(m_row.group(1)), int(m_row.group(2)) % cols
            screen[r] = screen[r][-by:] + screen[r][:-by] if by else screen[r]
            continue

        m_col = re.match(r"^rotate column x=(\d+) by (\d+)$", s)
        if m_col:
            c, by = int(m_col.group(1)), int(m_col.group(2)) % rows
            col_vals = [screen[r][c] for r in range(rows)]
            if by:
                col_vals = col_vals[-by:] + col_vals[:-by]
            for r in range(rows):
                screen[r][c] = col_vals[r]

    lit = sum(sum(row) for row in screen)
    rendered = "\n".join("".join("#" if cell else "." for cell in row) for row in screen)
    return lit, "\n" + rendered


if __name__ == "__main__":
    lines = Path("input_08.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 08 -> Parte 1: {p1} | Parte 2 (pantalla):{p2}")
