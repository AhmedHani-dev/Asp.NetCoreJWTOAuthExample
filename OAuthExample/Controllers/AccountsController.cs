using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthExample.DbContexts;
using OAuthExample.Entites;
using OAuthExample.Models.Requests;
using OAuthExample.Models.Responses;
using OAuthExample.Services;
using System.Security.Cryptography;
using System.Text;

namespace OAuthExample.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AccountsController(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }


    [HttpPost]
    public async Task<ActionResult<UserRegisterResponse>> Register([FromBody] RegisterUserRequest registerRequest)
    {
        if (await UserExists(registerRequest.UserName))
            return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();
        var user = new User()
        {
            UserName = registerRequest.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerRequest.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserRegisterResponse
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost]
    public async Task<ActionResult<UserLoginResponse>> Login(LoginUserRequest loginRequest)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginRequest.UserName);

        if (user == null)
            return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginRequest.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
                return Unauthorized("Invalid Password");
        }

        return new UserLoginResponse
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName.ToLower());
    }
}
