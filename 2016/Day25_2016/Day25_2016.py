from pathlib import Path


def solve(lines: list[str]):
    # Extrae constantes x e y del programa assembunny típico del Día 25:
    # cpy <x> c ... cpy <y> b ... (b += c * <y>)
    prog = [line.strip().split() for line in lines if line.strip()]
    x_val = int(prog[1][1]) if len(prog) > 1 and prog[1][1].isdigit() else 0
    y_val = int(prog[2][1]) if len(prog) > 2 and prog[2][1].isdigit() else 0
    base = x_val * y_val

    target_pattern = [0, 1] * 12  # 24 bits alternantes 0, 1, 0, 1...
    target_num = 0
    for bit in reversed(target_pattern):
        target_num = (target_num << 1) | bit

    a = 0
    while True:
        val = a + base
        # Revisa si los primeros 24 bits generados por LFSR/multiplicador coinciden con 0,1,0,1...
        out = []
        n = val
        while len(out) < len(target_pattern):
            # En AoC 2016 Día 25 el bucle interno suma 2550 (o 365 * 7) a n
            n += 2550
            temp = n
            while temp > 0 and len(out) < len(target_pattern):
                out.append(temp % 2)
                temp //= 2
        if out == target_pattern:
            return a, "Signal locked!"
        a += 1


if __name__ == "__main__":
    lines = Path("input_25.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 25 -> Parte 1: {p1} | Parte 2: {p2}")
