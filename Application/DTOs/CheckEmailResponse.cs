namespace Application.DTOs;

public record CheckEmailResponse
(
    bool Exists,
    string Email
);
