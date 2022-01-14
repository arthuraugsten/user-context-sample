using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserContextSample.Services;

public sealed record UserInfo(string Nome, Guid DepartmentId);

public interface IContextResolver
{
    public UserInfo GetUser();
}

public sealed class UserContext
{
    private readonly IContextResolver _contextResolver;
    private UserInfo? _userInfo;

    public UserContext(IContextResolver contextResolver)
    {
        _contextResolver = contextResolver;
    }

    public UserInfo User => _userInfo ??= _contextResolver.GetUser();
}

public sealed class HttpContextResolver : IContextResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextResolver(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public UserInfo GetUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var name = user?.Identity?.Name ?? "Anonymous";
        var departmentId = Guid.Parse(user.FindFirstValue(CustomClaims.DepartmentId));

        // query by http request the extra user data.

        return new(name, departmentId);
    }
}

public sealed class TestHttpContextResolver : IContextResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TestHttpContextResolver(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public UserInfo GetUser()
    {
        _ = _httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
        var contextoHttp = _httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(_httpContextAccessor));

        var headerUser = contextoHttp.Request?.Headers["UserForTest"] ?? StringValues.Empty;

        return headerUser switch
        {
            var h when h != StringValues.Empty =>
                JsonSerializer.Deserialize<UserInfo>(Encoding.UTF8.GetString(Convert.FromBase64String(headerUser)))!,
            _ => new HttpContextResolver(_httpContextAccessor).GetUser()
        };
    }
}
