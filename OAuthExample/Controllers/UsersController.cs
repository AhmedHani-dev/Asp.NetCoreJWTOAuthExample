using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthExample.DbContexts;
using OAuthExample.Models.Responses;

namespace OAuthExample.Controllers;
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        return await _context.Users
            .AsNoTracking()
            .Select(u => new UserResponse() { Id = u.Id, Name = u.UserName })
            .ToListAsync();
    }
}
