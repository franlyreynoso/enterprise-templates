namespace EnterpriseTemplate.Domain.Abstractions;

/// <summary>
/// Interface for entities that support audit tracking of creation and modification
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// When the entity was created
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Who created the entity
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// When the entity was last updated
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Who last updated the entity
    /// </summary>
    string UpdatedBy { get; set; }
}
