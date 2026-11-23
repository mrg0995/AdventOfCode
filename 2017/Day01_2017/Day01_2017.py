from pathlib import Path

def solve_captcha(digits: str, step: int) -> int:
    length = len(digits)
    if length == 0:
        return 0

    return sum(
        int(digits[i])
        for i in range(length)
        if digits[i] == digits[(i + step) % length]
    )

def main() -> None:
    raw_input = Path(r"Your_Route").read_text(encoding="utf-8").strip()

    if not raw_input or not raw_input.isdigit():
        raise ValueError("La entrada debe contener solo dígitos.")

    part1 = solve_captcha(raw_input, step=1)
    part2 = solve_captcha(raw_input, step=len(raw_input) // 2)

    print(f"Parte 1: {part1}")
    print(f"Parte 2: {part2}")

if __name__ == "__main__":
    main()