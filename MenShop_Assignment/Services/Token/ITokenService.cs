using MenShop_Assignment.Datas;

namespace MenShop_Assignment.Services.Token
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user, IList<string> roles);
    }
}
