using Application.Abstractions;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class IdentityService(UserManager<AppUser> userManager) : IIdentityService
{
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null){
            return false;
        }

        return true;
    }
}