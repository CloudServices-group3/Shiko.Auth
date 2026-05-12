namespace Application.DTOs;

public sealed record AccessTokenResult
(
    string AccessToken,
    string TokenType,
    int Expires,
    DateTime ExpiresAtUtc
);
