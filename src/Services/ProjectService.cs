using FocusMark.App.Cli.Models;
using FocusMark.App.Cli.Models.Project;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> logger;
        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService authorizationService;
        private readonly ApiConfiguration apiConfiguration;

        public ProjectService(ILogger<ProjectService> logger, IConfiguration configuration, IHttpClientFactory clientFactory, IAuthorizationService authorizationService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));

            this.apiConfiguration = new ApiConfiguration();
            configuration.GetSection("FocusMark").Bind(this.apiConfiguration);
        }

        public async Task<ApiResponse<string>> CreateProject(CreateProjectRequest createRequest)
        {
            JwtTokens jwtTokens = await this.authorizationService.GetTokens();

            HttpClient httpClient = this.clientFactory.CreateClient(nameof(ProjectModel));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtTokens.AccessToken);

            string apiUrl = $"{this.apiConfiguration.ApiUrl}{this.apiConfiguration.ProjectPath}";

            var domainCommand = new MediaTypeHeaderValue("application/json");
            domainCommand.Parameters.Add(new NameValueHeaderValue(ApiCommands.CommandKey, ApiCommands.CreateProjectCommand));
            HttpContent httpBody = new StringContent(JsonConvert.SerializeObject(createRequest));
            httpBody.Headers.ContentType = domainCommand;

            HttpResponseMessage postResponse = await httpClient.PostAsync(apiUrl, httpBody);
            string jsonPayload = await postResponse.Content.ReadAsStringAsync();
            if (postResponse.StatusCode == HttpStatusCode.Accepted)
            {
                ApiResponse<string> apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(jsonPayload);
                return apiResponse;
            }
            else if (postResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                throw new ApiUnavailableException("Project");
            }
        }
    }
}
