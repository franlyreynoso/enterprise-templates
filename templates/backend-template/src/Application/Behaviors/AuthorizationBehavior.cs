using EnterpriseTemplate.Application.Abstractions;
using MediatR;

namespace EnterpriseTemplate.Application.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizableRequest
{
    private readonly ICurrentUser _user;
    public AuthorizationBehavior(ICurrentUser user) => _user = user;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var perm in request.RequiredPermissions)
            if (!_user.Has(perm)) throw new UnauthorizedAccessException($"Missing permission: {perm}");

        return await next();
    }
}
