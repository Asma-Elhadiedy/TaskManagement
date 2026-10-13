
namespace TaskManagement.Application.Features.StaticData.Queries.TaskStatuses;

public sealed record GetTaskStatusesQuery : IRequest<Result<IReadOnlyList<StaticDataResponse>>>;
