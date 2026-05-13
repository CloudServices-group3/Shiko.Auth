using System.Globalization;

namespace Application.DTOs;

public sealed record RegisterAuthRequest
(
      string Email,
      string Password
);