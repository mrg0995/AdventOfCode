from pathlib import Path

def is_valid_no_duplicates(passphrase: str) -> bool:
    words = passphrase.split()
    return len(words) == len(set(words))

def is_valid_no_anagrams(passphrase: str) -> bool:
    words = passphrase.split()
    canonical_words = ["".join(sorted(word)) for word in words]
    return len(canonical_words) == len(set(canonical_words))

def main() -> None:
    lines = [
        line
        for line in Path(r"C:\Users\ProtoMkUser\Desktop\Proyectos\AdventOfCode\2017\Day04_2017\Day04_2017.txt").read_text(encoding="utf-8").splitlines()
        if line.strip()
    ]

    part1 = sum(1 for line in lines if is_valid_no_duplicates(line))
    part2 = sum(1 for line in lines if is_valid_no_anagrams(line))

    print(f"Parte 1: {part1}")
    print(f"Parte 2: {part2}")

if __name__ == "__main__":
    main()