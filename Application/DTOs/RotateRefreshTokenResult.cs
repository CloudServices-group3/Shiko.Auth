namespace Application.DTOs;

public record RotateRefreshTokenResult
(
    bool Succeeded,
    string? UserId,
    string? NewRefreshToken
)
{
    public static RotateRefreshTokenResult Failed() => new(false, null, null);

    public static RotateRefreshTokenResult Success(string userId, string newRefreshToken)
            => new(true, userId, newRefreshToken);
};
