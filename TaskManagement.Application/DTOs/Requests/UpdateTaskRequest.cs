
namespace TaskManagement.Application.DTOs.Requests;

public record UpdateTaskRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate);