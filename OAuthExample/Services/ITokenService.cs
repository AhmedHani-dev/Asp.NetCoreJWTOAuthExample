using OAuthExample.Entites;

namespace OAuthExample.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
