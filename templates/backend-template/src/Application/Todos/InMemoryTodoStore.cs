namespace EnterpriseTemplate.Application.Todos;

public interface ITodoStore
{
    Task<Todo> AddAsync(Todo todo, CancellationToken ct);
    Task<IReadOnlyList<Todo>> ListByOwnerAsync(string ownerId, CancellationToken ct);
}

public sealed class InMemoryTodoStore : ITodoStore
{
    private readonly List<Todo> _items = new();
    public Task<Todo> AddAsync(Todo todo, CancellationToken ct)
    {
        _items.Add(todo);
        return Task.FromResult(todo);
    }

    public Task<IReadOnlyList<Todo>> ListByOwnerAsync(string ownerId, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<Todo>>(_items.Where(t => t.OwnerId == ownerId).ToList());
}
