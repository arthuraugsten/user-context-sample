namespace UserContextSample;

public sealed record User(string Name, Guid DepartmentId);

public sealed record Todo(string Name)
{
    public bool Open { get; private set; } = true;

    public void Close() => Open = false;
}
