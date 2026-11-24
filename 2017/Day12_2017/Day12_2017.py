from collections import deque
from pathlib import Path
import sys

def parse_graph(lines: list[str]) -> dict[int, list[int]]:
    graph: dict[int, list[int]] = {}
    for line in lines:
        line = line.strip()
        if not line:
            continue
        left, right = line.split("<->", 1)
        node = int(left.strip())
        neighbors = [int(x.strip()) for x in right.split(",") if x.strip()]
        graph[node] = neighbors
    return graph

def collect_component(start: int, graph: dict[int, list[int]], visited: set[int]) -> set[int]:
    comp: set[int] = set()
    queue: deque[int] = deque([start])
    visited.add(start)

    while queue:
        curr = queue.popleft()
        comp.add(curr)
        for nxt in graph.get(curr, []):
            if nxt not in visited:
                visited.add(nxt)
                queue.append(nxt)

    return comp

def solve_pipes(graph: dict[int, list[int]]) -> tuple[int, int]:
    visited: set[int] = set()
    group_zero_size = len(collect_component(0, graph, visited)) if 0 in graph else 0
    groups = 1 if group_zero_size > 0 else 0

    for node in graph:
        if node not in visited:
            collect_component(node, graph, visited)
            groups += 1

    return group_zero_size, groups

def main() -> None:
    path = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("input12.txt")
    lines = path.read_text(encoding="utf-8").splitlines() if path.exists() else []
    graph = parse_graph(lines)
    part1, part2 = solve_pipes(graph)
    print(f"Día 12 - Parte 1: {part1}")
    print(f"Día 12 - Parte 2: {part2}")

if __name__ == "__main__":
    main()