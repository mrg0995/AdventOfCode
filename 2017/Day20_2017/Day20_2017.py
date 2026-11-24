from collections import Counter
from pathlib import Path
import re
import sys

PATTERN = re.compile(
    r"p=<\s*(-?\d+),\s*(-?\d+),\s*(-?\d+)>,\s*"
    r"v=<\s*(-?\d+),\s*(-?\d+),\s*(-?\d+)>,\s*"
    r"a=<\s*(-?\d+),\s*(-?\d+),\s*(-?\d+)>"
)

def parse_particles(lines: list[str]) -> list[list[int]]:
    particles: list[list[int]] = []
    for line in lines:
        m = PATTERN.search(line)
        if m:
            particles.append([int(x) for x in m.groups()])
    return particles

def manhattan(x: int, y: int, z: int) -> int:
    return abs(x) + abs(y) + abs(z)

def solve_part1(particles: list[list[int]]) -> int:
    best_idx = 0
    best_key = None
    for idx, (px, py, pz, vx, vy, vz, ax, ay, az) in enumerate(particles):
        key = (manhattan(ax, ay, az), manhattan(vx, vy, vz), manhattan(px, py, pz))
        if best_key is None or key < best_key:
            best_key = key
            best_idx = idx
    return best_idx

def solve_part2(particles: list[list[int]]) -> int:
    active = [p[:] for p in particles]
    quiet_ticks = 0

    while quiet_ticks < 200 and len(active) > 1:
        for p in active:
            p[3] += p[6]
            p[4] += p[7]
            p[5] += p[8]
            p[0] += p[3]
            p[1] += p[4]
            p[2] += p[5]

        counts = Counter((p[0], p[1], p[2]) for p in active)
        if any(cnt > 1 for cnt in counts.values()):
            active = [p for p in active if counts[(p[0], p[1], p[2])] == 1]
            quiet_ticks = 0
        else:
            quiet_ticks += 1

    return len(active)

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input20.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    particles = parse_particles(lines)
    print(f"Día 20 - Parte 1: {solve_part1(particles)}")
    print(f"Día 20 - Parte 2: {solve_part2(particles)}")

if __name__ == "__main__":
    main()