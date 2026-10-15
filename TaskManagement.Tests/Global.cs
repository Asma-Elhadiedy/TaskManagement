

global using Moq;
global using Newtonsoft.Json;
global using TaskManagement.Domain.UoW;
global using TaskManagement.Domain.Enums;
global using TaskManagement.Domain.Entities;
global using TaskManagement.Domain.Repository;
global using Microsoft.AspNetCore.Mvc.Testing;
global using TaskManagement.Application.Interfaces;
global using TaskManagement.Application.DTOs.Requests;
global using TaskManagement.Application.Features.Users.Commands.Login;
global using TaskManagement.Application.Features.Users.Commands.Register;
global using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
global using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
global using TaskManagement.Application.Features.Comments.Commands.CreateComment;