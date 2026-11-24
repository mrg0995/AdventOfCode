from functools import reduce
from itertools import combinations
import operator
from pathlib import Path

def solve(lines: list[str]):
    weights = sorted([int(x.strip()) for x in lines if x.strip()], reverse=True)

    def can_partition_rest(rem_weights, target):
        rem_weights = sorted(rem_weights, reverse=True)
        for k in range(1, len(rem_weights)):
            for comb in combinations(rem_weights, k):
                if sum(comb) == target:
                    return True
        return False

    def min_qe(groups: int):
        total = sum(weights)
        target = total // groups
        for k in range(1, len(weights)):
            candidates = []
            for comb in combinations(weights, k):
                if sum(comb) == target:
                    qe = reduce(operator.mul, comb, 1)
                    candidates.append((qe, set(comb)))
            candidates.sort(key=lambda x: x[0])
            for qe, used in candidates:
                leftover = [w for w in weights if w not in used]
                if groups == 2 or can_partition_rest(leftover, target):
                    return qe
        return None

    return min_qe(3), min_qe(4)

if __name__ == "__main__":
    lines = Path("input_24.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 24 -> Parte 1: {p1} | Parte 2: {p2}")
