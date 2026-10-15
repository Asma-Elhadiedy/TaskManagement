
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using TaskManagement.Application.DTOs.Responses;
using System.Text.Json;
using TaskManagement.Application.Common;

namespace TaskManagement.Tests.IntegrationTests;

public class UserTaskCreationFlowTests(WebApplicationFactory<Program> _factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = _factory.CreateClient();
    private record ApiResult<T>(bool IsSuccess, T? Data, string? Error);


    [Fact]
    public async Task User_CanRegister_Login_CreateProject_CreateTask_UpdateStatus()
    {
        var email = $"user_{Guid.NewGuid():N}@example.com";
        const string password = "Password123!";


        #region 1. Register
        var register = await _client.PostAsJsonAsync("/api/users/register",
            new RegisterUserCommand("User", "Test", email, password));
        register.EnsureSuccessStatusCode();
        #endregion



        #region 2. Login -> token
        var login = await _client.PostAsJsonAsync("/api/users/login",
            new LoginUserCommand(email, password));


        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<ApiResult<LoginUserResponse>>(
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.Data.Token);
        #endregion



        #region 3. Create project -> projectId
        var projectResponse = await _client.PostAsJsonAsync("/api/projects",
                new CreateProjectRequest("Test Project", "Test Description",
                    DateTime.UtcNow, DateTime.UtcNow.AddDays(60)));

        projectResponse.EnsureSuccessStatusCode();
        var projectIdResponse = await projectResponse.Content.ReadFromJsonAsync<JsonElement>();
        var projectId = projectIdResponse.GetProperty("projectId").GetGuid();
        //Assert
        Assert.NotEqual(Guid.Empty, projectId);
        #endregion



        #region 4. Create task -> taskId
        var taskResponse = await _client.PostAsJsonAsync($"/api/projects/{projectId}/tasks",
            new CreateTaskRequest("Test Task", "This is a test task.",
                DateTime.UtcNow, DateTime.UtcNow.AddDays(7)));

        taskResponse.EnsureSuccessStatusCode();
        var taskIdResponse = await taskResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = taskIdResponse.GetProperty("taskId").GetGuid();
        
        //Assert
        Assert.NotEqual(Guid.Empty, taskId);
        #endregion



        #region 5. Update status 
        var statusResponse = await _client.PutAsync(
            $"/api/projects/{projectId}/tasks/{taskId}/status?newStatus={(int)ETaskStatus.InProgress}",
            null);
        statusResponse.EnsureSuccessStatusCode();
        #endregion

    
    }

}
