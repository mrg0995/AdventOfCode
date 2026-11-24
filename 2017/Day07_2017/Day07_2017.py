from pathlib import Path
import re
import sys

def parse_programs(lines: list[str]) -> tuple[dict[str, int], dict[str, list[str]]]:
    weights: dict[str, int] = {}
    children: dict[str, list[str]] = {}
    pattern = re.compile(r"^([a-z]+)\s+\((\d+)\)(?:\s+->\s+(.+))?$")

    for line in lines:
        line = line.strip()
        if not line:
            continue
        match = pattern.match(line)
        if not match:
            raise ValueError(f"Línea inválida: {line}")
        name = match.group(1)
        weight = int(match.group(2))
        kids = [c.strip() for c in match.group(3).split(",")] if match.group(3) else []
        weights[name] = weight
        children[name] = kids

    return weights, children

def find_root(weights: dict[str, int], children: dict[str, list[str]]) -> str:
    all_children = {child for kids in children.values() for child in kids}
    for name in weights:
        if name not in all_children:
            return name
    raise ValueError("No se encontró raíz.")

def find_corrected_weight(
    root: str, weights: dict[str, int], children: dict[str, list[str]]
) -> int:
    corrected_target: int | None = None

    def tower_weight(node: str) -> int:
        nonlocal corrected_target
        kids = children[node]
        if not kids:
            return weights[node]

        child_totals = [(kid, tower_weight(kid)) for kid in kids]
        if corrected_target is None:
            freq: dict[int, int] = {}
            for _, total in child_totals:
                freq[total] = freq.get(total, 0) + 1

            if len(freq) > 1:
                minority_weight = next(w for w, count in freq.items() if count == 1)
                majority_weight = next(w for w, count in freq.items() if count > 1)
                bad_kid, bad_total = next(
                    (kid, total) for kid, total in child_totals if total == minority_weight
                )
                delta = majority_weight - bad_total
                corrected_target = weights[bad_kid] + delta

        return weights[node] + sum(total for _, total in child_totals)

    tower_weight(root)
    if corrected_target is None:
        raise ValueError("No se detectó desequilibrio.")
    return corrected_target

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input07.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    weights, children = parse_programs(lines)
    root = find_root(weights, children)
    target = find_corrected_weight(root, weights, children)
    print(f"Día 7 - Parte 1 (raíz): {root}")
    print(f"Día 7 - Parte 2 (peso corregido): {target}")

if __name__ == "__main__":
    main()