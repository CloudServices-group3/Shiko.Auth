using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName));

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT Configuration is missing");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.GetSigningKey(), // See method in JwtOptions.cs file.

                    ValidateLifetime = true,

                    /* 
                     * ClockSkew sets a "margin" for whether the system should accept a Token or not. Because servers do not always run at exactly the same time.
                     * In this case, the "margin" is 30 seconds. So a 29 second old key is still accepted. 
                     */
                    ClockSkew = TimeSpan.FromSeconds(30),

                    NameClaimType = JwtClaimTypes.Email, // Which claim counts as a username.
                    RoleClaimType = JwtClaimTypes.Role, // Which claim counts as roles.
                };
            });

        services.AddAuthorization();

        services.AddScoped<JwtTokenService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<RefreshTokenHasher>();

        return services;
    }
}
