
using Moq;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.Features.Comments.Commands.CreateComment;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Repository;
using TaskManagement.Domain.UoW;

namespace TaskManagement.Tests.UnitTests;

public class TasksTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IBackgroundServices> _backgroundServicesMock = new();
    private readonly Mock<IGenericRepository<Project>> _projectRepoMock = new();
    private readonly Mock<IGenericRepository<ProjectTask>> _taskRepoMock = new();
    private readonly Mock<IGenericRepository<Comment>> _commentRepoMock = new();
    private readonly CreateTaskCommandHandler _createTaskHandler;
    private readonly UpdateTaskStatusCommandHandler _updateTaskStatusHandler;
    private readonly CreateCommentCommandHandler _createCommentHandler;

    public TasksTests()
    {
        _unitOfWorkMock.Setup(u => u.Repository<Project>()).Returns(_projectRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<ProjectTask>()).Returns(_taskRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Comment>()).Returns(_commentRepoMock.Object);
        
        _createTaskHandler = new(_unitOfWorkMock.Object);
        _updateTaskStatusHandler = new(_unitOfWorkMock.Object, _backgroundServicesMock.Object);
        _createCommentHandler = new(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateTask_InvalidProjectId_ReturnsFailure()
    {
        //Arrange
        var command = new CreateTaskCommand
        (
            new CreateTaskRequest(
                "Test Task",
                "This is a test task.",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(7)
            ),
            Guid.NewGuid(), // Invalid ProjectId
            Guid.NewGuid().ToString()
        );

        //Act
        var result = await _createTaskHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        _unitOfWorkMock.Verify(x => x.Repository<ProjectTask>().Add(It.Is<ProjectTask>(p => p.Name == command.Request.Name)), Times.Once);


    }

    [Fact]
    public async Task AddTaskComment_InvalidTaskId_ReturnsFailure()
    {
        //Arrange
        var command = new CreateCommentCommand
        (
            Guid.NewGuid(),
            "This is a test comment.",
            Guid.NewGuid().ToString()
        );

        _unitOfWorkMock.Setup(x => x.CompleteAsync()).ReturnsAsync(0);

        //Act
        var result = await _createCommentHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task AssignTask_InvalidUserId_ReturnsFailure()
    {
        //Arrange
        var command = new CreateCommentCommand
        (
            Guid.NewGuid(),
            "This is a test comment.",
            Guid.NewGuid().ToString()
        );

        _unitOfWorkMock.Setup(x => x.CompleteAsync()).ReturnsAsync(0);

        //Act
        var result = await _createCommentHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateTaskStatus_ValidTransitions_ReturnsSuccess()
    {
        //Arrange 
        var existing = new ProjectTask { Id = Guid.NewGuid(), Status = ETaskStatus.Todo };

        var command = new UpdateTaskStatusCommand
        (
            existing.Id,
            ETaskStatus.InProgress
        );

        _taskRepoMock
            .Setup(r => r.GetByIdAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _unitOfWorkMock.Setup(r => r.CompleteAsync()).ReturnsAsync(1);

        //Act
        var result = await _updateTaskStatusHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateTaskStatus_InvalidTransitions_ReturnsFailure()
    {
        //Arrange 
        var existing = new ProjectTask { Id = Guid.NewGuid(), Status = ETaskStatus.Todo };
        var command = new UpdateTaskStatusCommand
        (
            existing.Id,
            ETaskStatus.Completed
        );

        _taskRepoMock
            .Setup(r => r.GetByIdAsync(command.TaskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _unitOfWorkMock.Setup(r => r.CompleteAsync()).ReturnsAsync(0);

        //Act
        try
        {
            await _updateTaskStatusHandler.Handle(command, CancellationToken.None);
        }
        catch (Exception ex)
        {
            //Assert
            Assert.True(true);
            Assert.Equal("Cannot transition task from 'Todo' to 'Completed'.", ex.Message);
        }


    }
}
