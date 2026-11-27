from pathlib import Path

BASE_PATTERN = (0, 1, 0, -1)

def fft_phase_p1(signal: list[int]) -> list[int]:
    n = len(signal)
    out = [0] * n
    for i in range(n):
        rep = i + 1
        total = 0
        for j in range(n):
            pat_idx = ((j + 1) // rep) % 4
            coeff = BASE_PATTERN[pat_idx]
            if coeff:
                total += signal[j] * coeff
        out[i] = abs(total) % 10
    return out

def fft_phase_second_half(tail: list[int]) -> list[int]:
    running = 0
    out = [0] * len(tail)
    for i in range(len(tail) - 1, -1, -1):
        running = (running + tail[i]) % 10
        out[i] = running
    return out

def solve(raw: str) -> tuple[str, str]:
    digits = [int(ch) for ch in raw.strip() if ch.isdigit()]

    sig1 = digits.copy()
    for _ in range(100):
        sig1 = fft_phase_p1(sig1)
    p1 = "".join(str(d) for d in sig1[:8])

    offset = int("".join(str(d) for d in digits[:7]))
    extended = digits * 10_000
    assert offset >= len(extended) // 2, "Expected offset in second half of signal"

    tail = extended[offset:]
    for _ in range(100):
        tail = fft_phase_second_half(tail)
    p2 = "".join(str(d) for d in tail[:8])

    return p1, p2

if __name__ == "__main__":
    raw = Path("Day16_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 16 - Part 1: {p1}, Part 2: {p2}")