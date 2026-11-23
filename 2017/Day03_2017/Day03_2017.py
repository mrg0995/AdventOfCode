from pathlib import Path

def manhattan_distance(target: int) -> int:
    if target <= 1:
        return 0

    ring = 0
    side = 1
    while side * side < target:
        side += 2
        ring += 1

    max_val = side * side
    step = side - 1

    min_offset = min(
        abs(target - (max_val - ring - i * step))
        for i in range(4)
    )
    return ring + min_offset

def first_stress_test_value_greater_than(target: int) -> int:
    grid: dict[tuple[int, int], int] = {(0, 0): 1}

    def sum_neighbors(cx: int, cy: int) -> int:
        return sum(
            grid.get((cx + dx, cy + dy), 0)
            for dx in (-1, 0, 1)
            for dy in (-1, 0, 1)
            if not (dx == 0 and dy == 0)
        )

    x, y = 1, 0
    layer = 1

    while True:
        while y < layer:
            val = sum_neighbors(x, y)
            grid[(x, y)] = val
            if val > target:
                return val
            y += 1

        while x > -layer:
            val = sum_neighbors(x, y)
            grid[(x, y)] = val
            if val > target:
                return val
            x -= 1

        while y > -layer:
            val = sum_neighbors(x, y)
            grid[(x, y)] = val
            if val > target:
                return val
            y -= 1

        while x <= layer:
            val = sum_neighbors(x, y)
            grid[(x, y)] = val
            if val > target:
                return val
            x += 1

        layer += 1

def main() -> None:
    raw = 361527
    target = int(raw)

    print(f"Parte 1: {manhattan_distance(target)}")
    print(f"Parte 2: {first_stress_test_value_greater_than(target)}")

if __name__ == "__main__":
    main()