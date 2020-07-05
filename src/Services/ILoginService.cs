using FocusMark.App.Cli.Models;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public interface ILoginService
    {
        Task<JwtTokens> Login();
    }
}
