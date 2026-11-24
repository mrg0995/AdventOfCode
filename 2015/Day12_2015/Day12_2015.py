import json
from pathlib import Path

def solve(raw_json: str):
    data = json.loads(raw_json)

    def sum_nums(obj, ignore_red=False):
        if isinstance(obj, int):
            return obj
        if isinstance(obj, list):
            return sum(sum_nums(x, ignore_red) for x in obj)
        if isinstance(obj, dict):
            if ignore_red and ("red" in obj.keys() or "red" in obj.values()):
                return 0
            return sum(sum_nums(v, ignore_red) for v in obj.values())
        return 0

    return sum_nums(data, False), sum_nums(data, True)

if __name__ == "__main__":
    raw = Path("input_12.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Día 12 -> Parte 1: {p1} | Parte 2: {p2}")
