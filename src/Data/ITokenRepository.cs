using FocusMark.App.Cli.Models;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Data
{
    public interface ITokenRepository
    {
        Task SaveTokens(JwtTokens tokens);

        Task<JwtTokens> GetTokens();

        Task DeleteTokens();
    }
}
