using System.Security.Claims;

namespace ExpenseManagment.Data.Common
{
    public interface IAuthenticatedUserAccessor
    {
        public string UserId { get; }
        public string Username { get; }
    }

    public class AuthenticatedUserAccessor : IAuthenticatedUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Lazy<string> _userIdLazy;
        private Lazy<string> _usernameLazy;

        public AuthenticatedUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userIdLazy = new Lazy<string>(() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            _usernameLazy = new Lazy<string>(() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "");
        }

        public string UserId => _userIdLazy.Value;
        public string Username => _usernameLazy.Value;
    }
}
