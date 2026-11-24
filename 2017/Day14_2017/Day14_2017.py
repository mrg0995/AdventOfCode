from collections import deque

def knot_hash(raw_input: str) -> str:
    lengths = [ord(ch) for ch in raw_input.strip()] + [17, 31, 73, 47, 23]
    values = list(range(256))
    pos = 0
    skip = 0

    for _ in range(64):
        for length in lengths:
            for i in range(length // 2):
                a = (pos + i) % 256
                b = (pos + length - 1 - i) % 256
                values[a], values[b] = values[b], values[a]
            pos = (pos + length + skip) % 256
            skip += 1

    dense: list[int] = []
    for block in range(16):
        xor_val = 0
        for i in range(16):
            xor_val ^= values[block * 16 + i]
        dense.append(xor_val)

    return "".join(f"{byte:02x}" for byte in dense)

def build_grid(key: str) -> list[list[bool]]:
    grid: list[list[bool]] = []
    for row in range(128):
        h = knot_hash(f"{key}-{row}")
        bits = f"{int(h, 16):0128b}"
        grid.append([ch == "1" for ch in bits])
    return grid

def count_regions(grid: list[list[bool]]) -> int:
    rows, cols = len(grid), len(grid[0])
    visited = [[False] * cols for _ in range(rows)]
    regions = 0
    dirs = [(-1, 0), (1, 0), (0, -1), (0, 1)]

    for r in range(rows):
        for c in range(cols):
            if not grid[r][c] or visited[r][c]:
                continue
            regions += 1
            queue: deque[tuple[int, int]] = deque([(r, c)])
            visited[r][c] = True

            while queue:
                cr, cc = queue.popleft()
                for dr, dc in dirs:
                    nr, nc = cr + dr, cc + dc
                    if 0 <= nr < rows and 0 <= nc < cols and grid[nr][nc] and not visited[nr][nc]:
                        visited[nr][nc] = True
                        queue.append((nr, nc))

    return regions

def main() -> None:
    key = "uugsqrei"
    grid = build_grid(key)
    part1 = sum(cell for row in grid for cell in row)
    part2 = count_regions(grid)
    print(f"Día 14 - Parte 1: {part1}")
    print(f"Día 14 - Parte 2: {part2}")

if __name__ == "__main__":
    main()