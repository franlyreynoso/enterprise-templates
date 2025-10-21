using EnterpriseTemplate.Domain.Common;

namespace EnterpriseTemplate.Application.Todos;

/// <summary>
/// Todo entity with audit tracking and soft delete capabilities
/// </summary>
public sealed class Todo : AuditableSoftDeletableEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public string OwnerId { get; set; } = string.Empty;

    // Parameterless constructor for EF Core
    public Todo() { }

    // Constructor for creating new todos
    public Todo(Guid id, string title, bool isDone, string ownerId)
    {
        Id = id;
        Title = title;
        IsDone = isDone;
        OwnerId = ownerId;
    }
}
