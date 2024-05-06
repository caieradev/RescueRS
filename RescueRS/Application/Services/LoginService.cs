using Microsoft.AspNetCore.Mvc;
using ResgateRS.Presenter.Controllers.App.V1.Enums;
using ResgateRS.Domain.Application.Entities;
using ResgateRS.Domain.Application.Services.Interfaces;
using ResgateRS.Infrastructure.Repositories;
using ResgateRS.Presenter.Controllers.App.V1.DTOs;
using ResgateRS.Tools;
using ResgateRS.Auth;

namespace ResgateRS.Domain.Application.Services;

public class LoginService(IServiceProvider serviceProvider, UserSession userSession) : BaseService(serviceProvider, userSession), IService
{
    public async Task<ActionResult<LoginResponseDTO>> handle(LoginRequestDTO dto)
    {
        if (string.IsNullOrEmpty(dto.Cellphone))
            return new BadRequestObjectResult(new ResponseDTO
            {
                DebugMessage = "Telefone é necessário",
                Message = "An error occurred, try again!"
            });

        UserEntity? user = await _serviceProvider.GetRequiredService<UserRepository>().GetUser(dto);

        if (user == null)
        {
            user = new UserEntity
            {
                Cellphone = dto.Cellphone,
                Rescuer = dto.Rescuer,
            };

            user = await _serviceProvider.GetRequiredService<UserRepository>().InsertOrUpdate(user);
        }


        var userSession = new UserSession
        {
            UserId = user.UserId.ToString(),
            Rescuer = user.Rescuer,
            Cellphone = user.Cellphone
        };

        var jwt = _serviceProvider.GetRequiredService<JwtTool>();
        jwt.setUserData(userSession);
        string token = jwt.generateToken();

        return new OkObjectResult(new LoginResponseDTO
        {
            Token = token,
            Rescuer = user.Rescuer
        });
    }
}