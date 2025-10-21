namespace EnterpriseTemplate.Application.Abstractions;

public interface IAuthorizableRequest
{
    IReadOnlyList<string> RequiredPermissions { get; }
}
