from pathlib import Path

def parse_program(text: str):
    lines = [line.strip() for line in text.strip().splitlines() if line.strip()]
    ip_reg = int(lines[0].split()[1])
    op_map = {
        "addr": 0, "addi": 1, "mulr": 2, "muli": 3,
        "banr": 4, "bani": 5, "borr": 6, "bori": 7,
        "setr": 8, "seti": 9, "gtir": 10, "gtri": 11,
        "gtrr": 12, "eqir": 13, "eqri": 14, "eqrr": 15,
    }
    raw_insts = []
    compiled_insts = []
    for line in lines[1:]:
        op, a, b, c = line.split()
        ia, ib, ic = int(a), int(b), int(c)
        raw_insts.append((op, ia, ib, ic))
        compiled_insts.append((op_map[op], ia, ib, ic))
    return ip_reg, raw_insts, compiled_insts

def extract_prng_constants(raw_insts: list) -> tuple[int, int, int] | None:
    seed = None
    mul_val = None
    mask_val = None

    for op, a, b, _ in raw_insts:
        if op == "seti" and a > 10000:
            seed = a
        elif op == "muli" and b > 1000:
            mul_val = b
        elif op == "bani" and b > 1000000:
            mask_val = b

    if seed is not None and mul_val is not None and mask_val is not None:
        return seed, mul_val, mask_val
    return None

def generate_states_fast(seed: int, mul_val: int, mask_val: int):
    seen: set[int] = set()
    state = 0

    while True:
        acc = state | 0x10000
        state = seed

        while acc > 0:
            byte = acc & 0xFF
            state = (((state + byte) & mask_val) * mul_val) & mask_val
            acc >>= 8

        if state in seen:
            return
        seen.add(state)
        yield state

def step_fast(op: int, a: int, b: int, c: int, r: list[int]) -> None:
    if op == 0:
        r[c] = r[a] + r[b]
    elif op == 1:
        r[c] = r[a] + b
    elif op == 2:
        r[c] = r[a] * r[b]
    elif op == 3:
        r[c] = r[a] * b
    elif op == 4:
        r[c] = r[a] & r[b]
    elif op == 5:
        r[c] = r[a] & b
    elif op == 6:
        r[c] = r[a] | r[b]
    elif op == 7:
        r[c] = r[a] | b
    elif op == 8:
        r[c] = r[a]
    elif op == 9:
        r[c] = a
    elif op == 10:
        r[c] = 1 if a > r[b] else 0
    elif op == 11:
        r[c] = 1 if r[a] > b else 0
    elif op == 12:
        r[c] = 1 if r[a] > r[b] else 0
    elif op == 13:
        r[c] = 1 if a == r[b] else 0
    elif op == 14:
        r[c] = 1 if r[a] == b else 0
    elif op == 15:
        r[c] = 1 if r[a] == r[b] else 0

def find_halt_ip_and_reg(raw_insts: list) -> tuple[int, int]:
    for idx, (op, a, b, _) in enumerate(raw_insts):
        if op == "eqrr":
            if a == 0:
                return idx, b
            if b == 0:
                return idx, a
    raise ValueError("No se encontró instrucción eqrr comparando con r[0]")

def solve_vm_fallback(ip_reg: int, raw_insts: list, compiled_insts: list) -> tuple[int, int]:
    halt_ip, check_reg = find_halt_ip_and_reg(raw_insts)
    r = [0, 0, 0, 0, 0, 0]
    ip = 0
    n_inst = len(compiled_insts)
    seen: set[int] = set()
    first_val: int | None = None
    last_val: int | None = None

    while 0 <= ip < n_inst:
        r[ip_reg] = ip
        if ip == halt_ip:
            val = r[check_reg]
            if first_val is None:
                first_val = val
            if val in seen:
                break
            seen.add(val)
            last_val = val

        op, a, b, c = compiled_insts[ip]
        step_fast(op, a, b, c, r)
        ip = r[ip_reg] + 1

    assert first_val is not None and last_val is not None
    return first_val, last_val

def solve(raw_input: str) -> tuple[int, int]:
    ip_reg, raw_insts, compiled_insts = parse_program(raw_input)
    consts = extract_prng_constants(raw_insts)

    if consts is not None:
        seed, mul_val, mask_val = consts
        first_val: int | None = None
        last_val: int | None = None

        for val in generate_states_fast(seed, mul_val, mask_val):
            if first_val is None:
                first_val = val
            last_val = val

        if first_val is not None and last_val is not None:
            return first_val, last_val

    return solve_vm_fallback(ip_reg, raw_insts, compiled_insts)

p1, p2 = solve(Path("input.txt").read_text(encoding="utf-8"))
print(f"Day 21 -> Part 1: {p1} | Part 2: {p2}")