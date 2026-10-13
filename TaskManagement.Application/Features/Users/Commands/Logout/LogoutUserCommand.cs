
namespace TaskManagement.Application.Features.Users.Commands.Logout;

public sealed record LogoutUserCommand(string UserId) : IRequest;
