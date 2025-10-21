using EnterpriseTemplate.Application.Abstractions;
using FluentValidation;
using MediatR;

namespace EnterpriseTemplate.Application.Todos;

public sealed record CreateTodo(string Title) : IRequest<Todo>, IAuthorizableRequest
{
    public IReadOnlyList<string> RequiredPermissions => new[] { "Todos.Create" };
}

public sealed class CreateTodoValidator : AbstractValidator<CreateTodo>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateTodoHandler : IRequestHandler<CreateTodo, Todo>
{
    private readonly ITodoStore _store;
    private readonly ICurrentUser _user;

    public CreateTodoHandler(ITodoStore store, ICurrentUser user)
    {
        _store = store; _user = user;
    }

    public async Task<Todo> Handle(CreateTodo request, CancellationToken ct)
    {
        var todo = new Todo(Guid.NewGuid(), request.Title, false, _user.Id ?? "anonymous");
        return await _store.AddAsync(todo, ct);
    }
}
