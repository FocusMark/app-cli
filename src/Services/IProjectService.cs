using FocusMark.App.Cli.Models;
using FocusMark.App.Cli.Models.Project;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public interface IProjectService
    {
        Task<ApiResponse<CreateProjectResponse>> CreateProject(CreateProjectRequest createRequest);
    }
}
