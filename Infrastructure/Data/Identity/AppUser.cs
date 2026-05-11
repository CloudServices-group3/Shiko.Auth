
// A "custom" user to be able to extend "IdentityUser" at a later time if needed.

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity;

public class AppUser : IdentityUser
{
}
