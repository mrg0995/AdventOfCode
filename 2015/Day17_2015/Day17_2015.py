from pathlib import Path

def solve(lines: list[str], liters: int = 150):
    containers = sorted([int(x.strip()) for x in lines if x.strip()], reverse=True)
    ways = []

    def dfs(idx, rem, count):
        if rem == 0:
            ways.append(count)
            return
        if idx == len(containers) or rem < 0:
            return
        dfs(idx + 1, rem - containers[idx], count + 1)
        dfs(idx + 1, rem, count)

    dfs(0, liters, 0)
    min_c = min(ways)
    return len(ways), sum(1 for c in ways if c == min_c)

if __name__ == "__main__":
    lines = Path("input_17.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 17 -> Parte 1: {p1} | Parte 2: {p2}")
