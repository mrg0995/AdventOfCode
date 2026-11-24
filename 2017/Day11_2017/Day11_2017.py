from pathlib import Path
import sys

HEX_DIRS: dict[str, tuple[int, int, int]] = {
    "n": (0, 1, -1),
    "ne": (1, 0, -1),
    "se": (1, -1, 0),
    "s": (0, -1, 1),
    "sw": (-1, 0, 1),
    "nw": (-1, 1, 0),
}

def hex_distance(x: int, y: int, z: int) -> int:
    return (abs(x) + abs(y) + abs(z)) // 2

def trace_hex_path(steps: list[str]) -> tuple[int, int]:
    x = y = z = 0
    max_dist = 0

    for step in steps:
        dx, dy, dz = HEX_DIRS[step]
        x += dx
        y += dy
        z += dz
        dist = hex_distance(x, y, z)
        if dist > max_dist:
            max_dist = dist

    return hex_distance(x, y, z), max_dist

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input11.txt")
    raw = path.read_text(encoding="utf-8").strip() if path.exists() else ""
    steps = [s.strip() for s in raw.split(",") if s.strip()]
    part1, part2 = trace_hex_path(steps)
    print(f"Día 11 - Parte 1: {part1}")
    print(f"Día 11 - Parte 2: {part2}")

if __name__ == "__main__":
    main()