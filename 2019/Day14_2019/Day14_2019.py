from collections import defaultdict
import math
from pathlib import Path

def parse_term(term: str) -> tuple[str, int]:
    qty_s, chem = term.strip().split()
    return chem, int(qty_s)

def solve(lines: list[str]) -> tuple[int, int]:
    reactions: dict[str, tuple[int, list[tuple[str, int]]]] = {}
    for line in lines:
        line = line.strip()
        if not line:
            continue
        lhs, rhs = line.split("=>")
        out_chem, out_qty = parse_term(rhs)
        inputs = [parse_term(part) for part in lhs.split(",")]
        reactions[out_chem] = (out_qty, inputs)

    def ore_for_fuel(fuel_amount: int) -> int:
        ore_needed = 0
        surplus: dict[str, int] = defaultdict(int)
        need: dict[str, int] = defaultdict(int, {"FUEL": fuel_amount})

        while need:
            chem, req = next(iter(need.items()))
            del need[chem]

            if chem == "ORE":
                ore_needed += req
                continue

            if surplus[chem] >= req:
                surplus[chem] -= req
                continue

            req -= surplus[chem]
            surplus[chem] = 0

            out_qty, inputs = reactions[chem]
            batches = math.ceil(req / out_qty)
            produced = batches * out_qty
            surplus[chem] += produced - req

            for in_chem, in_qty in inputs:
                need[in_chem] += batches * in_qty

        return ore_needed

    p1 = ore_for_fuel(1)

    total_ore = 1_000_000_000_000
    lo, hi = 1, total_ore
    best = 1
    while lo <= hi:
        mid = (lo + hi) // 2
        if ore_for_fuel(mid) <= total_ore:
            best = mid
            lo = mid + 1
        else:
            hi = mid - 1

    return p1, best

if __name__ == "__main__":
    lines = Path("Day14_2019.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Day 14 - Part 1: {p1}, Part 2: {p2}")