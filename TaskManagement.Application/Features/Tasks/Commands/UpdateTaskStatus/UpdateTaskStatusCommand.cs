
namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public sealed record UpdateTaskStatusCommand(Guid TaskId, ETaskStatus NewStatus) : IRequest<Result<bool>>;
