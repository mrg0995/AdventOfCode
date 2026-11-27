from math import gcd, lcm
from pathlib import Path
import re

def step_axis(pos: list[int], vel: list[int]) -> None:
    n = len(pos)
    for i in range(n):
        for j in range(i + 1, n):
            if pos[i] < pos[j]:
                vel[i] += 1
                vel[j] -= 1
            elif pos[i] > pos[j]:
                vel[i] -= 1
                vel[j] += 1
    for i in range(n):
        pos[i] += vel[i]

def axis_cycle(init_pos: list[int]) -> int:
    pos = init_pos.copy()
    vel = [0] * len(pos)
    steps = 0
    while True:
        step_axis(pos, vel)
        steps += 1
        if pos == init_pos and all(v == 0 for v in vel):
            return steps

def solve(lines: list[str]) -> tuple[int, int]:
    moons = []
    for line in lines:
        nums = [int(x) for x in re.findall(r"-?\d+", line)]
        if len(nums) == 3:
            moons.append(nums)

    px = [m[0] for m in moons]
    py = [m[1] for m in moons]
    pz = [m[2] for m in moons]
    vx = [0] * len(moons)
    vy = [0] * len(moons)
    vz = [0] * len(moons)

    for _ in range(1000):
        step_axis(px, vx)
        step_axis(py, vy)
        step_axis(pz, vz)

    p1 = sum(
        (abs(px[i]) + abs(py[i]) + abs(pz[i])) * (abs(vx[i]) + abs(vy[i]) + abs(vz[i]))
        for i in range(len(moons))
    )

    cx = axis_cycle([m[0] for m in moons])
    cy = axis_cycle([m[1] for m in moons])
    cz = axis_cycle([m[2] for m in moons])
    p2 = lcm(cx, cy, cz)
    return p1, p2

if __name__ == "__main__":
    lines = Path("Day12_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 12 - Part 1: {p1}, Part 2: {p2}")