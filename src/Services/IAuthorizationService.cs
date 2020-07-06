using FocusMark.App.Cli.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public interface IAuthorizationService
    {
        Task<bool> IsUserAuthorized();

        Task<JwtTokens> GetTokens();

        Task DeauthorizeUser();

        Task<bool> RefreshAuthorization();

        Task<bool> AuthorizeUser();
    }
}
