using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var inputPath = args.Length > 0 ? args[0] : "input.txt";
var rawText = File.Exists(inputPath)
    ? File.ReadAllText(inputPath)
    : GetSampleBlueprint();

var blueprint = ParseTuringBlueprint(rawText);
var checksum = RunDiagnosticChecksum(blueprint);

Console.WriteLine($"Parte 1 - Diagnostic checksum tras {blueprint.Steps} pasos: {checksum}");
Console.WriteLine("Parte 2 - Completada al reunir las estrellas anteriores (sin cálculo adicional en el Día 25).");

static TuringBlueprint ParseTuringBlueprint(string text)
{
    var lines = text
        .Split(["\r\n", "\n"], StringSplitOptions.None)
        .Select(l => l.Trim())
        .Where(l => !string.IsNullOrEmpty(l))
        .ToArray();

    if (lines.Length < 2)
        throw new FormatException("La entrada no contiene cabecera válida.");

    var startMatch = Regex.Match(lines[0], @"Begin in state ([A-Z]+)\.", RegexOptions.IgnoreCase);
    var stepsMatch = Regex.Match(lines[1], @"Perform a diagnostic checksum after (\d+) steps\.", RegexOptions.IgnoreCase);

    if (!startMatch.Success || !stepsMatch.Success)
        throw new FormatException("No se pudo leer el estado inicial o el número de pasos.");

    var initialState = startMatch.Groups[1].Value.ToUpperInvariant();
    var steps = long.Parse(stepsMatch.Groups[1].Value, CultureInfo.InvariantCulture);

    var states = new Dictionary<string, StateRules>();
    var i = 2;

    while (i < lines.Length)
    {
        var stateMatch = Regex.Match(lines[i], @"In state ([A-Z]+):", RegexOptions.IgnoreCase);
        if (!stateMatch.Success)
        {
            i++;
            continue;
        }

        var stateName = stateMatch.Groups[1].Value.ToUpperInvariant();
        i++;

        var branch0 = ParseBranch(lines, ref i, expectedValue: 0);
        var branch1 = ParseBranch(lines, ref i, expectedValue: 1);

        states[stateName] = new StateRules(branch0, branch1);
    }

    return new TuringBlueprint(initialState, steps, states);
}

static TuringAction ParseBranch(string[] lines, ref int index, int expectedValue)
{
    if (index + 3 >= lines.Length)
        throw new FormatException($"Bloque incompleto para el valor {expectedValue}.");

    var condMatch = Regex.Match(lines[index], $@"If the current value is {expectedValue}:", RegexOptions.IgnoreCase);
    if (!condMatch.Success)
        throw new FormatException($"Se esperaba 'If the current value is {expectedValue}:' y se encontró '{lines[index]}'.");

    var writeMatch = Regex.Match(lines[index + 1], @"- Write the value ([01])\.", RegexOptions.IgnoreCase);
    var moveMatch = Regex.Match(lines[index + 2], @"- Move one slot to the (left|right)\.", RegexOptions.IgnoreCase);
    var nextMatch = Regex.Match(lines[index + 3], @"- Continue with state ([A-Z]+)\.", RegexOptions.IgnoreCase);

    if (!writeMatch.Success || !moveMatch.Success || !nextMatch.Success)
        throw new FormatException($"Acciones inválidas en el bloque del valor {expectedValue}.");

    var writeValue = int.Parse(writeMatch.Groups[1].Value, CultureInfo.InvariantCulture);
    var moveDelta = moveMatch.Groups[1].Value.Equals("right", StringComparison.OrdinalIgnoreCase) ? 1 : -1;
    var nextState = nextMatch.Groups[1].Value.ToUpperInvariant();

    index += 4;
    return new TuringAction(writeValue, moveDelta, nextState);
}

static int RunDiagnosticChecksum(TuringBlueprint blueprint)
{
    var ones = new HashSet<int>();
    var cursor = 0;
    var state = blueprint.InitialState;

    for (long step = 0; step < blueprint.Steps; step++)
    {
        var currentValue = ones.Contains(cursor) ? 1 : 0;
        var rules = blueprint.States[state];
        var action = currentValue == 0 ? rules.WhenZero : rules.WhenOne;

        if (action.WriteValue == 1)
            ones.Add(cursor);
        else
            ones.Remove(cursor);

        cursor += action.MoveDelta;
        state = action.NextState;
    }

    return ones.Count;
}

static string GetSampleBlueprint() =>
    """
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
    """;

readonly record struct TuringAction(int WriteValue, int MoveDelta, string NextState);
readonly record struct StateRules(TuringAction WhenZero, TuringAction WhenOne);
sealed record TuringBlueprint(string InitialState, long Steps, Dictionary<string, StateRules> States);