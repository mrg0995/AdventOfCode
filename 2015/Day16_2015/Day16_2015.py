from pathlib import Path

def solve(lines: list[str]):
    target = {
        "children": 3, "cats": 7, "samoyeds": 2, "pomeranians": 3,
        "akitas": 0, "vizslas": 0, "goldfish": 5, "trees": 3,
        "cars": 2, "perfumes": 1,
    }
    p1 = p2 = None
    for line in lines:
        line = line.strip()
        if not line:
            continue
        head, rest = line.split(": ", 1)
        num = int(head.split()[1])
        props = {}
        for item in rest.split(", "):
            k, v = item.split(": ")
            props[k] = int(v)

        if all(target[k] == v for k, v in props.items()):
            p1 = num

        ok2 = True
        for k, v in props.items():
            if k in ("cats", "trees"):
                if not (v > target[k]):
                    ok2 = False
            elif k in ("pomeranians", "goldfish"):
                if not (v < target[k]):
                    ok2 = False
            else:
                if v != target[k]:
                    ok2 = False
        if ok2:
            p2 = num

    return p1, p2

if __name__ == "__main__":
    lines = Path("input_16.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 16 -> Parte 1: {p1} | Parte 2: {p2}")
