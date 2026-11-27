from collections import defaultdict, deque
import copy
from itertools import combinations

class IntcodeVM:
    def __init__(self, program: list[int], inputs: list[int] | None = None):
        self.initial_program = list(program)
        self.reset(inputs)

    def reset(self, inputs: list[int] | None = None):
        self.mem: dict[int, int] = defaultdict(int, enumerate(self.initial_program))
        self.ip: int = 0
        self.rel_base: int = 0
        self.inputs: deque[int] = deque(inputs or [])
        self.outputs: list[int] = []
        self.halted: bool = False
        self.waiting_input: bool = False

    def _read(self, addr: int) -> int:
        if addr < 0:
            raise ValueError(f"Dirección de memoria negativa: {addr}")
        return self.mem[addr]

    def _write(self, addr: int, value: int):
        if addr < 0:
            raise ValueError(f"Escritura en dirección negativa: {addr}")
        self.mem[addr] = value

    def _get_param_addr(self, offset: int, mode: int) -> int:
        raw = self._read(self.ip + offset)
        if mode == 0:
            return raw
        if mode == 2:
            return self.rel_base + raw
        raise ValueError(f"Modo inválido para dirección de escritura: {mode}")

    def _get_param_val(self, offset: int, mode: int) -> int:
        raw = self._read(self.ip + offset)
        if mode == 0:
            return self._read(raw)
        if mode == 1:
            return raw
        if mode == 2:
            return self._read(self.rel_base + raw)
        raise ValueError(f"Modo de parámetro desconocido: {mode}")

    def run_until_pause(self, clear_outputs: bool = True) -> list[int]:
        if clear_outputs:
            self.outputs.clear()
        self.waiting_input = False

        while not self.halted:
            instruction = self._read(self.ip)
            opcode = instruction % 100
            m1 = (instruction // 100) % 10
            m2 = (instruction // 1000) % 10
            m3 = (instruction // 10000) % 10

            if opcode == 1:
                a = self._get_param_val(1, m1)
                b = self._get_param_val(2, m2)
                dst = self._get_param_addr(3, m3)
                self._write(dst, a + b)
                self.ip += 4

            elif opcode == 2:
                a = self._get_param_val(1, m1)
                b = self._get_param_val(2, m2)
                dst = self._get_param_addr(3, m3)
                self._write(dst, a * b)
                self.ip += 4

            elif opcode == 3:
                if not self.inputs:
                    self.waiting_input = True
                    return list(self.outputs)
                dst = self._get_param_addr(1, m1)
                self._write(dst, self.inputs.popleft())
                self.ip += 2

            elif opcode == 4:
                val = self._get_param_val(1, m1)
                self.outputs.append(val)
                self.ip += 2

            elif opcode == 5:
                cond = self._get_param_val(1, m1)
                target = self._get_param_val(2, m2)
                self.ip = target if cond != 0 else self.ip + 3

            elif opcode == 6:
                cond = self._get_param_val(1, m1)
                target = self._get_param_val(2, m2)
                self.ip = target if cond == 0 else self.ip + 3

            elif opcode == 7:
                a = self._get_param_val(1, m1)
                b = self._get_param_val(2, m2)
                dst = self._get_param_addr(3, m3)
                self._write(dst, 1 if a < b else 0)
                self.ip += 4

            elif opcode == 8:
                a = self._get_param_val(1, m1)
                b = self._get_param_val(2, m2)
                dst = self._get_param_addr(3, m3)
                self._write(dst, 1 if a == b else 0)
                self.ip += 4

            elif opcode == 9:
                offset = self._get_param_val(1, m1)
                self.rel_base += offset
                self.ip += 2

            elif opcode == 99:
                self.halted = True
                return list(self.outputs)

            else:
                raise ValueError(f"Opcode desconocido {opcode} en ip={self.ip}")

        return list(self.outputs)

def send_cmd(vm: IntcodeVM, cmd: str) -> str:
    for ch in cmd.strip() + "\n":
        vm.inputs.append(ord(ch))
    out = vm.run_until_pause()
    return "".join(chr(code) for code in out)

def play_and_solve(program: list[int]):
    vm = IntcodeVM(program)
    checkpoint_vm: IntcodeVM | None = None

    print(send_cmd(vm, "") if False else "".join(chr(x) for x in vm.run_until_pause()), end="")
    print("\n[Comandos extra: `save` en Security Checkpoint, `load` para volver, `solve <dir>` para probar peso, `reset`]\n")

    while not vm.halted:
        raw = input("> ").strip()
        if not raw:
            continue

        if raw == "reset":
            vm.reset()
            print("".join(chr(x) for x in vm.run_until_pause()), end="")
            continue

        if raw == "save":
            checkpoint_vm = copy.deepcopy(vm)
            print("[Estado guardado en checkpoint.]")
            continue

        if raw == "load":
            if checkpoint_vm is None:
                print("[No hay ningún checkpoint guardado todavía.]")
            else:
                vm = copy.deepcopy(checkpoint_vm)
                print("[Checkpoint restaurado.]")
            continue

        if raw.startswith("solve "):
            if checkpoint_vm is None:
                print("[Guarda primero con `save` estando en Security Checkpoint con tus objetos.]")
                continue
            move_dir = raw.split(maxsplit=1)[1].strip()
            inv_text = send_cmd(copy.deepcopy(checkpoint_vm), "inv")
            items = [
                line.removeprefix("- ").strip()
                for line in inv_text.splitlines()
                if line.startswith("- ")
            ]
            print(f"[Probando {2 ** len(items)} combinaciones de {items} hacia '{move_dir}'...]")

            for r in range(len(items) + 1):
                for keep in combinations(items, r):
                    test_vm = copy.deepcopy(checkpoint_vm)
                    keep_set = set(keep)
                    for item in items:
                        if item not in keep_set:
                            send_cmd(test_vm, f"drop {item}")
                    result = send_cmd(test_vm, move_dir)
                    if "Alert!" not in result and "Command?" not in result:
                        print("\n¡COMBINACIÓN ENCONTRADA! Objetos:", keep)
                        print(result)
                        return
            print("[Ninguna combinación pasó el control; revisa si dejaste algún objeto fuera o si falta alguno.]")
            continue

        print(send_cmd(vm, raw), end="")

if __name__ == "__main__":
    with open("Day25_2019.txt", encoding="utf-8") as f:
        prog = [int(x) for x in f.read().strip().split(",")]
    play_and_solve(prog)