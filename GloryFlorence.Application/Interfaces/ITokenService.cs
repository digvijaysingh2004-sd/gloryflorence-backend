using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
