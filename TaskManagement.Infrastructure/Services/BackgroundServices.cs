
namespace TaskManagement.Infrastructure.Services;

public class BackgroundServices(IBackgroundJobClient _backgroundJobs, ILogger<BackgroundServices> _logger) : IBackgroundServices
{

    public Task SendTaskCompletedNotificationAsync(string userId, string taskTitle, CancellationToken ct)
    {
        //var jobId = _backgroundJobs.Enqueue(()
        //          => _logger.LogInformation("Task completed: {UserId} - {TaskTitle}", userId, taskTitle));
        var jobId = _backgroundJobs.Enqueue<IBackgroundServices>(s
                  => s.ProcessTaskCompletedLogAsync(userId, taskTitle));

        _backgroundJobs.ContinueJobWith<IBackgroundServices>(jobId,
            s => s.ProcessNotificationSentLogAsync(userId, taskTitle),
            JobContinuationOptions.OnAnyFinishedState);

        return Task.CompletedTask;
    }

    [Queue("default")]
    public Task ProcessTaskCompletedLogAsync(string userId, string taskTitle)
    {
        _logger.LogInformation("Task completed: {UserId} - {TaskTitle}", userId, taskTitle);
        return Task.CompletedTask;
    }

    [Queue("default")]
    public Task ProcessNotificationSentLogAsync(string userId, string taskTitle)
    {
        _logger.LogInformation("Notification sent for task: {UserId} - {TaskTitle}", userId, taskTitle);
        return Task.CompletedTask;
    }
}
