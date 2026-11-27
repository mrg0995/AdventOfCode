from collections import Counter

def is_valid_p1(s: str) -> bool:
    has_double = False
    for i in range(len(s) - 1):
        if s[i] > s[i + 1]:
            return False
        if s[i] == s[i + 1]:
            has_double = True
    return has_double

def is_valid_p2(s: str) -> bool:
    for i in range(len(s) - 1):
        if s[i] > s[i + 1]:
            return False
    return 2 in Counter(s).values()

def solve(raw: str) -> tuple[int, int]:
    low, high = map(int, raw.strip().split("-"))
    p1 = 0
    p2 = 0
    for n in range(low, high + 1):
        s = str(n)
        if is_valid_p1(s):
            p1 += 1
        if is_valid_p2(s):
            p2 += 1
    return p1, p2

if __name__ == "__main__":
    raw = "123257-647015"
    p1, p2 = solve(raw)
    print(f"Day 4 - Part 1: {p1}, Part 2: {p2}")