from pathlib import Path
import sys

def process_stream(stream: str) -> tuple[int, int]:
    depth = 0
    total_score = 0
    garbage_chars = 0
    in_garbage = False
    skip_next = False

    for ch in stream:
        if skip_next:
            skip_next = False
            continue

        if ch == "!":
            skip_next = True
            continue

        if in_garbage:
            if ch == ">":
                in_garbage = False
            else:
                garbage_chars += 1
        else:
            if ch == "<":
                in_garbage = True
            elif ch == "{":
                depth += 1
            elif ch == "}":
                total_score += depth
                depth -= 1

    return total_score, garbage_chars

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input09.txt")
    stream = path.read_text(encoding="utf-8").strip() if path.exists() else "{}"
    part1, part2 = process_stream(stream)
    print(f"Día 9 - Parte 1 (puntuación): {part1}")
    print(f"Día 9 - Parte 2 (basura): {part2}")

if __name__ == "__main__":
    main()