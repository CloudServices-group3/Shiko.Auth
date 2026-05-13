namespace Application.DTOs;

public sealed record LoginAuthRequest
(
      string Email,
      string Password
);