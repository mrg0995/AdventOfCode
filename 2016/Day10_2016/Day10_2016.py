from pathlib import Path
import re


def solve(lines: list[str]):
    bots = {}
    outputs = {}
    chips_log = {}
    rules = {}

    def ensure_bot(b):
        if b not in bots:
            bots[b] = []
        if b not in chips_log:
            chips_log[b] = set()

    def give(target_type, target_id, val):
        if target_type == "bot":
            ensure_bot(target_id)
            bots[target_id].append(val)
            chips_log[target_id].add(val)
        else:
            outputs[target_id] = val

    for line in lines:
        s = line.strip()
        if not s:
            continue
        m_val = re.match(r"^value (\d+) goes to bot (\d+)$", s)
        if m_val:
            give("bot", int(m_val.group(2)), int(m_val.group(1)))
            continue

        m_rule = re.match(
            r"^bot (\d+) gives low to (bot|output) (\d+) and high to (bot|output) (\d+)$",
            s,
        )
        if m_rule:
            b_id = int(m_rule.group(1))
            ensure_bot(b_id)
            rules[b_id] = (
                (m_rule.group(2), int(m_rule.group(3))),
                (m_rule.group(4), int(m_rule.group(5))),
            )

    target_bot = None
    progressed = True
    while progressed:
        progressed = False
        for b_id, chips in list(bots.items()):
            if len(chips) == 2 and b_id in rules:
                progressed = True
                low_val, high_val = sorted(chips)
                bots[b_id].clear()
                if {low_val, high_val} == {17, 61}:
                    target_bot = b_id
                (l_type, l_id), (h_type, h_id) = rules[b_id]
                give(l_type, l_id, low_val)
                give(h_type, h_id, high_val)

    p2 = outputs.get(0, 0) * outputs.get(1, 0) * outputs.get(2, 0)
    return target_bot, p2


if __name__ == "__main__":
    lines = Path("input_10.txt").read_text(encoding="utf-8").splitlines()
    p1, p2 = solve(lines)
    print(f"Día 10 -> Parte 1: {p1} | Parte 2: {p2}")
