from pathlib import Path

WIDTH = 25
HEIGHT = 6
LAYER_SIZE = WIDTH * HEIGHT

def solve(raw: str) -> tuple[int, str]:
    digits = raw.strip()
    layers = [digits[i : i + LAYER_SIZE] for i in range(0, len(digits), LAYER_SIZE)]

    best_layer = min(layers, key=lambda l: l.count("0"))
    p1 = best_layer.count("1") * best_layer.count("2")

    final_pixels: list[str] = []
    for idx in range(LAYER_SIZE):
        pixel = "#"
        for layer in layers:
            if layer[idx] == "0":
                pixel = " "
                break
            if layer[idx] == "1":
                pixel = "#"
                break
        final_pixels.append(pixel)

    rendered_rows = [
        "".join(final_pixels[r * WIDTH : (r + 1) * WIDTH])
        for r in range(HEIGHT)
    ]
    return p1, "\n".join(rendered_rows)

if __name__ == "__main__":
    raw = Path("Day08_2019.txt").read_text(encoding="utf-8")
    p1, p2 = solve(raw)
    print(f"Day 8 - Part 1: {p1}\nDay 8 - Part 2:\n{p2}")