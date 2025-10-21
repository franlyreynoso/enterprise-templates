using EnterpriseTemplate.Application.Abstractions;
using MediatR;

namespace EnterpriseTemplate.Application.Todos;

public sealed record ListMyTodos : IRequest<IReadOnlyList<Todo>>, IAuthorizableRequest
{
    public IReadOnlyList<string> RequiredPermissions => new[] { "Todos.Read" };
}

public sealed class ListMyTodosHandler : IRequestHandler<ListMyTodos, IReadOnlyList<Todo>>
{
    private readonly ITodoStore _store;
    private readonly ICurrentUser _user;

    public ListMyTodosHandler(ITodoStore store, ICurrentUser user)
    {
        _store = store; _user = user;
    }

    public Task<IReadOnlyList<Todo>> Handle(ListMyTodos request, CancellationToken ct)
        => _store.ListByOwnerAsync(_user.Id ?? "anonymous", ct);
}
