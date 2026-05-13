using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data.Identity;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Shiko.Auth.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapPost("/refresh", Refresh);
        group.MapPost("/logout", Logout);
        group.MapGet("/me", Me).RequireAuthorization(); // An endpoint for the frontend to be able to find out who is logged in.
    }

    private static async Task<IResult> Register(RegisterAuthRequest request, UserManager<AppUser> userManager)
    {
        // ToLowerInvariant() = Converts uppercase letters to lowercase letters without using language-specific rules from the user's computer.
        var email = request.Email.Trim().ToLowerInvariant();

        var user = new AppUser
        {
            UserName = email,
            Email = email,
        };

        var result = await userManager.CreateAsync(user, request.Password);


        // If User is not created the result.Errors contains Error message from ASP.NET Identity.
        // in ToDictionary we decide what should be the key and value to show frontend. ( Key-value pairs ).
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(x => x.Code, x => new[] { x.Description }));

        // If Created - returns HTTP-status 201 Created with location header and the created user in the response body. 
        return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email }); 
    }

    private static async Task<IResult> Login (LoginAuthRequest request, UserManager<AppUser> userManager, JwtTokenService jwtTokenService, RefreshTokenService refreshTokenService, HttpContext httpContext, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Results.Unauthorized();

        if (await userManager.IsLockedOutAsync(user))
            return Results.Problem(title: "Locked out", detail: "User is temprarily locked out", statusCode: StatusCodes.Status423Locked);

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            await userManager.AccessFailedAsync(user);
            return Results.Unauthorized();
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = jwtTokenService.CreateAccessToken(user, roles); // Create Access token

        // httpContext.Connection.RemoteIpAddress?.ToString() = Get the IP address from the client making the request.
        var refreshToken = await refreshTokenService.CreateAsync(user.Id, httpContext.Connection.RemoteIpAddress?.ToString(), ct); // Create refresh token.


        // Returns HTTP 200 OK with an anonymous object that will be serialized to JSON.
        // This acts as an implicit response contract (not a DTO class)
        return Results.Ok(new
        {
            accessToken = accessToken.AccessToken,
            accessToken.TokenType,
            accessToken.Expires,
            accessToken.ExpiresAtUtc,
            refreshToken,

            // Anonym nested object.
            user = new
            {
                user = user.Id,
                email = user.Email,
                roles
            }
        });
    }

    private static async Task<IResult> Refresh(RefreshAuthRequest request, UserManager<AppUser> userManager, JwtTokenService jwtTokenService, RefreshTokenService refreshTokenService, HttpContext httpContext, CancellationToken ct = default)
    {
        // httpContext.Connection.RemoteIpAddress?.ToString() = Get the IP address from the client making the request.

        var result = await refreshTokenService.RotateAsync(request.RefreshToken, httpContext.Connection.RemoteIpAddress?.ToString(), ct);

        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.UserId))
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(result.UserId);
        if (user is null)
            return Results.Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenService.CreateAccessToken(user, roles);

        return Results.Ok(new
        {
            accessToken = accessToken.AccessToken,
            accessToken.TokenType,
            accessToken.Expires,
            accessToken.ExpiresAtUtc,
            refreshToken = result.NewRefreshToken,
            user = new
            {
                user = user.Id,
                email = user.Email,
                roles
            }
        });
    }

    private static async Task<IResult> Logout(LogoutAuthRequest request, RefreshTokenService refreshTokenService, HttpContext httpContext, CancellationToken ct = default)
    {
        // Disables/invalidates the refresh token so that it is no longer valid.

        await refreshTokenService.RevokeAsync(request.RefreshToken, httpContext.Connection.RemoteIpAddress?.ToString(), ct);

        return Results.NoContent();
    }

    [Authorize]
    private static async Task<IResult> Me(HttpContext httpContext, UserManager<AppUser> userManager, CancellationToken ct = default)
    {
        // HttpContext = all information about the request + user + connection and metadata.

        var userId = httpContext.User.FindFirst(JwtClaimTypes.UserId)?.Value;
        var email = httpContext.User.FindFirst(JwtClaimTypes.Email)?.Value;
        var roles = httpContext.User.FindAll(JwtClaimTypes.Role).Select(x => x.Value).ToArray();
        if (string.IsNullOrWhiteSpace(userId))
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Results.Unauthorized();

        return Results.Ok(new
        {
            user.Id,
            user.Email,
            user.PhoneNumber,
            roles
        });
    }
}
