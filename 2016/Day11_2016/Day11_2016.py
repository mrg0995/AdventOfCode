from collections import deque
from itertools import combinations
from pathlib import Path
import re


def solve(lines: list[str]):
    def parse(extra_pairs: int = 0):
        # Cada objeto es (elemento, 'G' o 'M')
        floors = [set() for _ in range(4)]
        for idx, line in enumerate(lines[:4]):
            for elem in re.findall(r"([a-z]+)-compatible microchip", line):
                floors[idx].add((elem, "M"))
            for elem in re.findall(r"([a-z]+) generator", line):
                floors[idx].add((elem, "G"))
        for i in range(extra_pairs):
            elem = f"elerium_{i}" if i == 0 else f"dilithium_{i}"
            floors[0].add((elem, "G"))
            floors[0].add((elem, "M"))
        return tuple(frozenset(f) for f in floors)

    def is_valid_floor(floor):
        gens = {elem for elem, kind in floor if kind == "G"}
        if not gens:
            return True
        for elem, kind in floor:
            if kind == "M" and elem not in gens:
                return False
        return True

    def canonical_state(elev, floors):
        # Normaliza nombres de elementos por su (piso_gen, piso_chip) para reducir estados equivalentes
        pos = {}
        for f_idx, floor in enumerate(floors):
            for elem, kind in floor:
                g_f, m_f = pos.get(elem, (-1, -1))
                if kind == "G":
                    pos[elem] = (f_idx, m_f)
                else:
                    pos[elem] = (g_f, f_idx)
        pairs_sig = tuple(sorted(pos.values()))
        return (elev, pairs_sig)

    def bfs(start_floors):
        n_items = sum(len(f) for f in start_floors)
        start = (0, start_floors)
        q = deque([(0, start)])
        seen = {canonical_state(0, start_floors)}

        while q:
            steps, (elev, floors) = q.popleft()
            if len(floors[3]) == n_items:
                return steps

            cur_items = sorted(floors[elev])
            move_groups = list(combinations(cur_items, 2)) + [(x,) for x in cur_items]

            for nxt_elev in (elev - 1, elev + 1):
                if not (0 <= nxt_elev < 4):
                    continue
                # Optimización: no bajar ascensor si todos los pisos inferiores están vacíos
                if nxt_elev < elev and all(len(floors[i]) == 0 for i in range(nxt_elev + 1)):
                    continue

                for grp in move_groups:
                    grp_set = set(grp)
                    f_from = frozenset(floors[elev] - grp_set)
                    f_to = frozenset(floors[nxt_elev] | grp_set)
                    if not (is_valid_floor(f_from) and is_valid_floor(f_to)):
                        continue

                    nxt_floors = list(floors)
                    nxt_floors[elev] = f_from
                    nxt_floors[nxt_elev] = f_to
                    nxt_floors_t = tuple(nxt_floors)

                    sig = canonical_state(nxt_elev, nxt_floors_t)
                    if sig not in seen:
                        seen.add(sig)
                        q.append((steps + 1, (nxt_elev, nxt_floors_t)))

        return None

    p1 = bfs(parse(0))
    p2 = bfs(parse(2))
    return p1, p2


if __name__ == "__main__":
    lines = Path("input_11.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 11 -> Parte 1: {p1} | Parte 2: {p2}")
