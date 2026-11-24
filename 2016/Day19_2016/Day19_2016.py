from pathlib import Path
import re


def solve(lines: list[str]):
    graph = {}
    for line in lines:
        m = re.match(r"^(\w+) <-> (.+)$", line.strip())
        if m:
            src, targets = m.group(1), [t.strip() for t in m.group(2).split(",")]
            graph[src] = targets

    def bfs_from(start: str) -> set[str]:
        seen = {start}
        stack = [start]
        while stack:
            cur = stack.pop()
            for nxt in graph.get(cur, []):
                if nxt not in seen:
                    seen.add(nxt)
                    stack.append(nxt)
        return seen

    p1 = len(bfs_from("0"))

    visited_all = set()
    groups = 0
    for node in graph:
        if node not in visited_all:
            groups += 1
            visited_all |= bfs_from(node)

    return p1, groups


if __name__ == "__main__":
    lines = Path("input_19.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 19 -> Parte 1: {p1} | Parte 2: {p2}")
