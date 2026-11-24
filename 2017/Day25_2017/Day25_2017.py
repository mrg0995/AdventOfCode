from pathlib import Path
import re


def parse_blueprint(text: str) -> tuple[str, int, dict[tuple[str, int], tuple[int, int, str]]]:
    start_match = re.search(r"Begin in state ([A-Z])\.", text)
    steps_match = re.search(r"Perform a diagnostic checksum after (\d+) steps\.", text)
    if not start_match or not steps_match:
        raise ValueError("No se pudo leer el estado inicial o el número de pasos.")

    initial_state = start_match.group(1)
    total_steps = int(steps_match.group(1))

    state_blocks = text.strip().split("\n\n")[1:]
    rules: dict[tuple[str, int], tuple[int, int, str]] = {}

    branch_pattern = re.compile(
        r"If the current value is ([01]):\s+"
        r"- Write the value ([01])\.\s+"
        r"- Move one slot to the (left|right)\.\s+"
        r"- Continue with state ([A-Z])\."
    )

    for block in state_blocks:
        state_match = re.search(r"In state ([A-Z]):", block)
        if not state_match:
            continue
        state = state_match.group(1)

        for cur_val_s, write_val_s, direction, next_state in branch_pattern.findall(block):
            cur_val = int(cur_val_s)
            write_val = int(write_val_s)
            move = 1 if direction == "right" else -1
            rules[(state, cur_val)] = (write_val, move, next_state)

    return initial_state, total_steps, rules


def run_turing_machine(text: str) -> int:
    state, total_steps, rules = parse_blueprint(text)
    tape: set[int] = set()
    cursor = 0

    for _ in range(total_steps):
        current_val = 1 if cursor in tape else 0
        write_val, move, next_state = rules[(state, current_val)]

        if write_val == 1:
            tape.add(cursor)
        else:
            tape.discard(cursor)

        cursor += move
        state = next_state

    return len(tape)


EXAMPLE_INPUT = """\
Begin in state A.
Perform a diagnostic checksum after 6 steps.

In state A:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state B.
  If the current value is 1:
    - Write the value 0.
    - Move one slot to the left.
    - Continue with state B.

In state B:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the left.
    - Continue with state A.
  If the current value is 1:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state A.
"""


if __name__ == "__main__":
    assert run_turing_machine(EXAMPLE_INPUT) == 3

    puzzle_input = Path("input25.txt").read_text(encoding="utf-8")
    print("Checksum:", run_turing_machine(puzzle_input))