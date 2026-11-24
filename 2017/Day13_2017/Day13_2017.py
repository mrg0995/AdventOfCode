from pathlib import Path
import sys

def parse_layers(lines: list[str]) -> list[tuple[int, int, int]]:
    layers: list[tuple[int, int, int]] = []
    for line in lines:
        line = line.strip()
        if not line:
            continue
        depth_str, range_str = line.split(":", 1)
        depth = int(depth_str.strip())
        rng = int(range_str.strip())
        period = 2 * (rng - 1) if rng > 1 else 1
        layers.append((depth, rng, period))
    return layers

def calculate_severity(layers: list[tuple[int, int, int]]) -> int:
    return sum(
        depth * rng
        for depth, rng, period in layers
        if depth % period == 0
    )

def find_safe_delay(layers: list[tuple[int, int, int]]) -> int:
    ordered_layers = sorted(layers, key=lambda item: item[2], reverse=True)

    delay = 0
    while True:
        if all((delay + depth) % period != 0 for depth, _, period in ordered_layers):
            return delay
        delay += 1

def main() -> None:
    input_path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input13.txt")
    if input_path.exists():
        lines = input_path.read_text(encoding="utf-8").splitlines()
    else:
        lines = [
            "0: 3",
            "1: 2",
            "4: 4",
            "6: 4",
        ]

    layers = parse_layers(lines)
    part1 = calculate_severity(layers)
    part2 = find_safe_delay(layers)

    print(f"Parte 1 - Severidad del recorrido (delay 0): {part1}")
    print(f"Parte 2 - Menor retraso seguro: {part2}")

if __name__ == "__main__":
    main()