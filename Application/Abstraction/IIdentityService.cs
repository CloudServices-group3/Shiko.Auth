using System.Globalization;

namespace Application.Abstraction;

public interface IIdentityService
{
    Task<string> CreateUserAsync(string email, CancellationToken ct = default);
    Task<string> CreatePasswordAsync(string userId, string password, CancellationToken ct = default);
}
