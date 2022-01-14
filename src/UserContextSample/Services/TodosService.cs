using UserContextSample.DomainEvents;

namespace UserContextSample.Services;

public sealed class TodosService
{
    private static readonly List<Todo> _todos = new(2) { new("Todo 1"), new("Todo 2") };
    private readonly UserContext _userContext;

    public TodosService(UserContext userContext)
    {
        _userContext = userContext;
    }

    public void CloseAllByDepartment(Guid departmentId)
    {
        // Use id for this query
        _todos.ForEach(todo => todo.Close());

        var @event = new AllTodosClosedDomainEvent(_todos, departmentId);
        // raise event
    }

    public IEnumerable<Todo> GetAllClosedByDepartment(Guid departmentId) => _todos; // Use if for this query

    public void CloseAllByDepartment()
    {
        var departmentId = _userContext.User.DepartmentId;
        // Use id for this query
        _todos.ForEach(todo => todo.Close());

        var @event = new AllTodosClosedDomainEvent(_todos, departmentId);
        // raise event
    }

    public IEnumerable<Todo> GetAllClosedByDepartment() => _todos; // Use id for this query (_userContext.User.DepartmentId)
}
