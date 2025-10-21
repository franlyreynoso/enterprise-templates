using Microsoft.EntityFrameworkCore;
using EnterpriseTemplate.Application.Todos;
using EnterpriseTemplate.Application.Abstractions;

namespace EnterpriseTemplate.Infrastructure.Persistence;

public sealed class EfTodoStore : ITodoStore
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _user;

    public EfTodoStore(AppDbContext db, ICurrentUser user)
    {
        _db = db; _user = user;
    }

    public async Task<Todo> AddAsync(Todo todo, CancellationToken ct)
    {
        await _db.Todos.AddAsync(todo, ct);
        await _db.SaveChangesAsync(ct);
        return todo;
    }

    public async Task<IReadOnlyList<Todo>> ListByOwnerAsync(string ownerId, CancellationToken ct)
        => await _db.Todos.Where(t => t.OwnerId == ownerId)
                          .OrderByDescending(t => t.Id)
                          .ToListAsync(ct);
}
