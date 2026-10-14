

namespace TaskManagement.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        //var taskOwnerId = await _unitOfWork.Repository<ProjectTask>()
        //    .GetItemSelectedAsync(
        //        t => t.UserId,
        //        t => t.Id == request.Request.TaskId,
        //        cancellationToken);

        //if (request.CreatedBy != taskOwnerId)
        //    return Result<Guid>.Failure("Another user owns this task, you can not create a comment.");

        _unitOfWork.Repository<Comment>().Add(new Comment
        {
            Content = request.Content,
            TaskId = request.TaskId,
            CreatedAt = DateTime.UtcNow,
            CreatedById = request.CreatedBy
        });

        return await _unitOfWork.CompleteAsync(cancellationToken) > 0
            ? Result<Guid>.Success(request.TaskId)
            : Result<Guid>.Failure("Failed to create comment.");
    }
}
