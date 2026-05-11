namespace Application.Abstractions;

public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email);
}
