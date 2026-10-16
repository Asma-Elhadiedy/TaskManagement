
namespace TaskManagement.Application.Interfaces;

public interface IBackgroundServices
{
    Task SendTaskCompletedNotificationAsync(string userId, string taskTitle, CancellationToken ct);

    Task ProcessTaskCompletedLogAsync(string userId, string taskTitle);
    Task ProcessNotificationSentLogAsync(string userId, string taskTitle);
}