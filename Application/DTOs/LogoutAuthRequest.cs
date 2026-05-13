namespace Application.DTOs;

public sealed record LogoutAuthRequest
(
    string RefreshToken
);