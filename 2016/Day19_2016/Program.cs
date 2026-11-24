using System.Diagnostics;

const int elfCount = 3005290;

var sw = Stopwatch.StartNew();

int part1 = SolvePart1(elfCount);
int part2 = SolvePart2(elfCount);

sw.Stop();

Console.WriteLine($"Parte 1 (roba a la izquierda): {part1}");
Console.WriteLine($"Parte 2 (roba al de enfrente):  {part2}");
Console.WriteLine($"Tiempo total: {sw.ElapsedMilliseconds} ms");

static int SolvePart1(int n)
{
    int highestPowerOfTwo = 1;
    while (highestPowerOfTwo * 2 <= n)
    {
        highestPowerOfTwo *= 2;
    }

    int remainder = n - highestPowerOfTwo;
    return 2 * remainder + 1;
}

static int SolvePart2(int n)
{
    var circle = new LinkedList<int>();
    for (int i = 1; i <= n; i++)
    {
        circle.AddLast(i);
    }

    int remaining = n;
    LinkedListNode<int> current = circle.First!;

    LinkedListNode<int> target = GetNodeAtOffset(circle, current, remaining / 2);

    while (remaining > 1)
    {
        LinkedListNode<int> nextTarget = GetNextCircular(circle, target);
        circle.Remove(target);
        remaining--;

        target = remaining % 2 == 1
            ? GetNextCircular(circle, nextTarget)
            : nextTarget;

        current = GetNextCircular(circle, current);
    }

    return current.Value;
}

static LinkedListNode<int> GetNextCircular(LinkedList<int> list, LinkedListNode<int> node)
    => node.Next ?? list.First!;

static LinkedListNode<int> GetNodeAtOffset(LinkedList<int> list, LinkedListNode<int> start, int steps)
{
    var node = start;
    for (int i = 0; i < steps; i++)
    {
        node = GetNextCircular(list, node);
    }
    return node;
}