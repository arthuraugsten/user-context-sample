namespace UserContextSample.DomainEvents;

public sealed record AllTodosClosedDomainEvent(IEnumerable<Todo> Todos, Guid DepartmenId);
