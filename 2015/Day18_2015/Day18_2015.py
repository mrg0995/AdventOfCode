from pathlib import Path

def solve(lines: list[str], steps: int = 100):
    grid = [list(line.strip()) for line in lines if line.strip()]
    h, w = len(grid), len(grid[0])

    def step_grid(g, corners_on):
        nxt = [row[:] for row in g]
        if corners_on:
            for r, c in ((0, 0), (0, w - 1), (h - 1, 0), (h - 1, w - 1)):
                g[r][c] = "#"
        for r in range(h):
            for c in range(w):
                if corners_on and (r, c) in ((0, 0), (0, w - 1), (h - 1, 0), (h - 1, w - 1)):
                    nxt[r][c] = "#"
                    continue
                on = 0
                for dr in (-1, 0, 1):
                    for dc in (-1, 0, 1):
                        if dr == 0 and dc == 0:
                            continue
                        nr, nc = r + dr, c + dc
                        if 0 <= nr < h and 0 <= nc < w and g[nr][nc] == "#":
                            on += 1
                if g[r][c] == "#":
                    nxt[r][c] = "#" if on in (2, 3) else "."
                else:
                    nxt[r][c] = "#" if on == 3 else "."
        return nxt

    g1 = [row[:] for row in grid]
    g2 = [row[:] for row in grid]
    for _ in range(steps):
        g1 = step_grid(g1, False)
        g2 = step_grid(g2, True)

    p1 = sum(row.count("#") for row in g1)
    p2 = sum(row.count("#") for row in g2)
    return p1, p2

if __name__ == "__main__":
    lines = Path("input_18.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 18 -> Parte 1: {p1} | Parte 2: {p2}")
