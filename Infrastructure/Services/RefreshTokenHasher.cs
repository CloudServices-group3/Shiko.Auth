using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class RefreshTokenHasher
{
    public string Hash(string token)
    {
        // Coverts token to bytes, hashes it with SHA-256 to create a secure value. Then encodes the result in Base64 so it can be stored and compared as text.

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
