from pathlib import Path

def parse_rows(text: str) -> list[list[int]]:
    rows: list[list[int]] = []
    for line in text.splitlines():
        if line.strip():
            rows.append([int(token) for token in line.split()])
    return rows

def row_difference(row: list[int]) -> int:
    return max(row) - min(row)

def row_evenly_divisible_quotient(row: list[int]) -> int:
    for i, a in enumerate(row):
        for j, b in enumerate(row):
            if i != j and b != 0 and a % b == 0:
                return a // b
    raise ValueError("No se encontró ningún par divisible de forma exacta en la fila.")

def main() -> None:
    raw_input = Path(r"Your_Route").read_text(encoding="utf-8")
    rows = parse_rows(raw_input)

    part1 = sum(row_difference(row) for row in rows)
    part2 = sum(row_evenly_divisible_quotient(row) for row in rows)

    print(f"Parte 1: {part1}")
    print(f"Parte 2: {part2}")

if __name__ == "__main__":
    main()