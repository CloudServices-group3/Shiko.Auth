using Application.DTOs;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;


namespace Shiko.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail(CheckEmailRequest request)
    {
        return Ok();
    }
}
