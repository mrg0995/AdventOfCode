from pathlib import Path

def solve(lines: list[str]):
    raw = {}
    for line in lines:
        line = line.strip()
        if " -> " in line:
            expr, target = line.split(" -> ")
            raw[target] = expr.split()

    def evaluate(wires):
        cache = {}

        def val(token):
            if token.isdigit():
                return int(token) & 0xFFFF
            if token in cache:
                return cache[token]
            parts = wires[token]
            if len(parts) == 1:
                res = val(parts[0])
            elif len(parts) == 2:  # NOT x
                res = (~val(parts[1])) & 0xFFFF
            else:
                a, op, b = parts
                if op == "AND":
                    res = val(a) & val(b)
                elif op == "OR":
                    res = val(a) | val(b)
                elif op == "LSHIFT":
                    res = (val(a) << val(b)) & 0xFFFF
                elif op == "RSHIFT":
                    res = (val(a) >> val(b)) & 0xFFFF
            cache[token] = res
            return res

        return val("a")

    p1 = evaluate(raw)
    modified = dict(raw)
    modified["b"] = [str(p1)]
    p2 = evaluate(modified)
    return p1, p2

if __name__ == "__main__":
    lines = Path("input_07.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 07 -> Parte 1: {p1} | Parte 2: {p2}")
