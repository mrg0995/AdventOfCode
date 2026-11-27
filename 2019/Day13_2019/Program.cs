using System.Globalization;

var inputPath = args.Length > 0 ? args[0] : "Day13_2019.txt";
var program = File.ReadAllText(inputPath)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(long.Parse)
    .ToArray();

Console.WriteLine($"Parte 1 (bloques iniciales): {SolvePart1(program)}");
Console.WriteLine($"Parte 2 (puntuación final):   {SolvePart2(program)}");

static long SolvePart1(long[] program)
{
    var vm = new IntcodeVm(program);
    var grid = new Dictionary<(long x, long y), long>();

    while (vm.StepUntilOutputOrHalt())
    {
        if (!vm.LastOutput.HasValue) break;
        long x = vm.LastOutput.Value;

        if (!vm.StepUntilOutputOrHalt() || !vm.LastOutput.HasValue) break;
        long y = vm.LastOutput.Value;

        if (!vm.StepUntilOutputOrHalt() || !vm.LastOutput.HasValue) break;
        long tileId = vm.LastOutput.Value;

        grid[(x, y)] = tileId;
    }

    return grid.Values.Count(t => t == 2);
}

static long SolvePart2(long[] program)
{
    var patched = (long[])program.Clone();
    patched[0] = 2;

    long ballX = 0;
    long paddleX = 0;
    long score = 0;

    var vm = new IntcodeVm(patched, inputProvider: () =>
    {
        if (ballX < paddleX) return -1;
        if (ballX > paddleX) return 1;
        return 0;
    });

    while (!vm.IsHalted)
    {
        if (!TryReadTriple(vm, out long x, out long y, out long value))
            break;

        if (x == -1 && y == 0)
        {
            score = value;
        }
        else if (value == 3)
        {
            paddleX = x;
        }
        else if (value == 4)
        {
            ballX = x;
        }
    }

    return score;
}

static bool TryReadTriple(IntcodeVm vm, out long x, out long y, out long z)
{
    x = y = z = 0;
    if (!vm.StepUntilOutputOrHalt() || !vm.LastOutput.HasValue) return false;
    x = vm.LastOutput.Value;

    if (!vm.StepUntilOutputOrHalt() || !vm.LastOutput.HasValue) return false;
    y = vm.LastOutput.Value;

    if (!vm.StepUntilOutputOrHalt() || !vm.LastOutput.HasValue) return false;
    z = vm.LastOutput.Value;

    return true;
}

sealed class IntcodeVm
{
    private readonly Dictionary<long, long> _mem = new();
    private readonly Func<long>? _inputProvider;
    private long _ip;
    private long _relBase;

    public bool IsHalted { get; private set; }
    public long? LastOutput { get; private set; }

    public IntcodeVm(ReadOnlySpan<long> initialMemory, Func<long>? inputProvider = null)
    {
        _inputProvider = inputProvider;
        for (long i = 0; i < initialMemory.Length; i++)
        {
            if (initialMemory[(int)i] != 0)
                _mem[i] = initialMemory[(int)i];
        }
    }

    private long Read(long addr) => _mem.TryGetValue(addr, out var v) ? v : 0;
    private void Write(long addr, long val) => _mem[addr] = val;

    private long GetParamAddress(long instruction, int index)
    {
        long divisor = index switch
        {
            1 => 100,
            2 => 1000,
            3 => 10000,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        long mode = (instruction / divisor) % 10;
        long raw = Read(_ip + index);

        return mode switch
        {
            0 => raw,
            1 => _ip + index,
            2 => _relBase + raw,
            _ => throw new InvalidOperationException($"Modo inválido: {mode}")
        };
    }

    private long GetParam(long instruction, int index) => Read(GetParamAddress(instruction, index));

    public bool StepUntilOutputOrHalt()
    {
        LastOutput = null;

        while (!IsHalted)
        {
            long inst = Read(_ip);
            int opcode = (int)(inst % 100);

            switch (opcode)
            {
                case 1:
                    Write(GetParamAddress(inst, 3), GetParam(inst, 1) + GetParam(inst, 2));
                    _ip += 4;
                    break;

                case 2:
                    Write(GetParamAddress(inst, 3), GetParam(inst, 1) * GetParam(inst, 2));
                    _ip += 4;
                    break;

                case 3:
                    if (_inputProvider is null)
                        throw new InvalidOperationException("El programa pidió entrada pero no hay inputProvider.");
                    Write(GetParamAddress(inst, 1), _inputProvider());
                    _ip += 2;
                    break;

                case 4:
                    LastOutput = GetParam(inst, 1);
                    _ip += 2;
                    return true;

                case 5:
                    _ip = GetParam(inst, 1) != 0 ? GetParam(inst, 2) : _ip + 3;
                    break;

                case 6:
                    _ip = GetParam(inst, 1) == 0 ? GetParam(inst, 2) : _ip + 3;
                    break;

                case 7:
                    Write(GetParamAddress(inst, 3), GetParam(inst, 1) < GetParam(inst, 2) ? 1 : 0);
                    _ip += 4;
                    break;

                case 8:
                    Write(GetParamAddress(inst, 3), GetParam(inst, 1) == GetParam(inst, 2) ? 1 : 0);
                    _ip += 4;
                    break;

                case 9:
                    _relBase += GetParam(inst, 1);
                    _ip += 2;
                    break;

                case 99:
                    IsHalted = true;
                    return false;

                default:
                    throw new InvalidOperationException($"Opcode desconocido: {opcode} en IP={_ip}");
            }
        }

        return false;
    }
}