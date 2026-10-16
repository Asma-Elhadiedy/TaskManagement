
using Quartz;

namespace TaskManagement.Infrastructure.Services;

[DisallowConcurrentExecution]

public class QuartzBackgroundServices(ILogger<QuartzBackgroundServices> _logger) : IJob<string>
{
   
    public async ValueTask Execute(IJobExecutionContext context, string taskTitle, CancellationToken cancellationToken = default)
    {
        await ProcessTaskCompletedLogAsync(taskTitle);
    }

    public Task ProcessTaskCompletedLogAsync(string taskTitle)
    {
        _logger.LogInformation("Task completed: {TaskTitle}", taskTitle);
        return Task.CompletedTask;
    }

    


}