namespace EnterpriseTemplate.Application.Abstractions;
public interface ICurrentUser
{
    string? Id { get; }
    bool Has(string permission);
}
