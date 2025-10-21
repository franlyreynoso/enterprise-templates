namespace EnterpriseTemplate.Domain.Abstractions;

/// <summary>
/// Interface for entities that support soft deletion
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// When the entity was deleted (null if not deleted)
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Who deleted the entity
    /// </summary>
    string? DeletedBy { get; set; }

    /// <summary>
    /// Whether the entity is deleted
    /// </summary>
    bool IsDeleted => DeletedAt.HasValue;
}
