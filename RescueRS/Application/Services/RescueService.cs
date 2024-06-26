﻿using Microsoft.AspNetCore.Mvc;
using ResgateRS.Presenter.Controllers.App.V1.Enums;
using ResgateRS.Domain.Application.Entities;
using ResgateRS.Domain.Application.Services.Interfaces;
using ResgateRS.Infrastructure.Repositories;
using ResgateRS.Presenter.Controllers.App.V1.DTOs;
using ResgateRS.Tools;
using ResgateRS.Auth;
using ResgateRS.DTOs;
using ResgateRS.Middleware;
using ResgateRS.Extensions;
using RescueRS.Application.Enums;

namespace ResgateRS.Domain.Application.Services;

public class RescueService(IServiceProvider serviceProvider, UserSession userSession) : BaseService(serviceProvider, userSession), IService
{
    public async Task<IResponse<object>> ConfirmRescue(RescueGuidDTO dto) =>
        await SetRescueStatus(dto, RescueStatusEnum.Completed);

    public async Task<IResponse<object>> StartRescue(RescueGuidDTO dto) =>
        await SetRescueStatus(dto, RescueStatusEnum.InProgress);

    public async Task<IResponse<object>> CancelRescue(RescueGuidDTO dto) =>
        await SetRescueStatus(dto, RescueStatusEnum.Pending);

    public async Task<IResponse<object>> SetRescueStatus(RescueGuidDTO dto, RescueStatusEnum status)
    {
        if (dto.RescueId == Guid.Empty)
            throw new Exception("RescueId is required");

        RescueEntity? rescue = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescueById(dto.RescueId) ??
            throw new MessageException("Não foi possível encontrar esse resgate.");

        if (_userSession.Rescuer == false && rescue.RequestedBy != _userSession.UserId)
            throw new MessageException("Você não tem permissão para confirmar este resgate");

        rescue.Status = status;
        rescue.UpdateDateTime = DateTimeOffset.Now;
        rescue.UpdatedBy = _userSession.UserId;

        switch (status)
        {
            case RescueStatusEnum.Completed:
                rescue.RescueDateTime = DateTimeOffset.Now;
                rescue.ConfirmedBy = _userSession.UserId;
                break;
            default:
                break;
        }

        await _serviceProvider.GetRequiredService<RescueRepository>().InsertOrUpdate(rescue);

        var message = status switch
        {
            RescueStatusEnum.Completed => "Resgate concluído!",
            RescueStatusEnum.InProgress => "Resgate iniciado!",
            RescueStatusEnum.Pending => "Resgate revertido para \"pendente\"!",
            _ => throw new Exception("Invalid status")
        };

        return Response.Success(default(Object), message);
    }

    public async Task<IResponse<RescueDTO>> DetailRescue(Guid rescueId)
    {
        if (rescueId == Guid.Empty)
            throw new Exception("RescueId is required");

        RescueEntity? rescue = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescueById(rescueId);

        if (rescue == null)
            throw new MessageException("Não foi possível encontrar esse resgate.");

        return Response.Success(RescueDTO.FromEntity(rescue, _userSession.UserId));
    }

    public async Task<IResponse<IEnumerable<RescueDTO>>> ListPendingRescues(double? latitude, double? longitude)
    {
        IEnumerable<RescueEntity> rescues = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescuesWithStatus(RescueStatusEnum.Pending);

        return Response.Success(rescues.Select(x => RescueDTO.FromEntity(x, _userSession.UserId, latitude, longitude)));
    }

    public async Task<IResponse<IEnumerable<RescueDTO>>> ListPendingRescuesByProximity(double latitude, double longitude)
    {
        if (latitude == 0 && longitude == 0)
            return await ListPendingRescues(null, null);

        IEnumerable<RescueEntity> rescues = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescuesByProximityWithStatus(RescueStatusEnum.Pending, latitude, longitude);

        return Response.Success(rescues.Select(x => RescueDTO.FromEntity(x, _userSession.UserId, latitude, longitude)));
    }

    public async Task<IResponse<IEnumerable<RescueDTO>>> ListCompletedRescues()
    {
        IEnumerable<RescueEntity> rescues = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescuesWithStatus(RescueStatusEnum.Completed);

        return Response.Success(rescues.Select(x => RescueDTO.FromEntity(x, _userSession.UserId)));
    }

    public async Task<IResponse<IEnumerable<RescueDTO>>> ListInProgressRescues()
    {
        IEnumerable<RescueEntity> rescues = await _serviceProvider.GetRequiredService<RescueRepository>().GetRescuesWithStatus(RescueStatusEnum.InProgress);

        return Response.Success(rescues.Select(x => RescueDTO.FromEntity(x, _userSession.UserId)));
    }

    public async Task<IResponse<IEnumerable<RescueDTO>>> ListMyRescues()
    {
        IEnumerable<RescueEntity> rescues = await _serviceProvider.GetRequiredService<RescueRepository>().GetMyRescues(_userSession.UserId, _userSession.Rescuer);

        return Response.Success(rescues.Select(x => RescueDTO.FromEntity(x, _userSession.UserId)));
    }

    public async Task<IResponse<object>> RequestRescue(RescueRequestDTO dto)
    {
        if (dto.ContactPhone != null && !dto.ContactPhone.IsValidCellphone())
            throw new MessageException("Número de telefone inválido.");

        if (_userSession.Rescuer)
            throw new Exception("Rescuer cannot request rescue");

        if (dto.AdultsNumber < 0 || dto.AnimalsNumber < 0 || dto.ChildrenNumber < 0 || dto.DisabledNumber < 0 || dto.ElderlyNumber < 0)
            throw new MessageException("Número negativo de pessoas não é permitido.");

        if (dto.AdultsNumber + dto.AnimalsNumber + dto.ChildrenNumber + dto.DisabledNumber + dto.ElderlyNumber <= 0)
            throw new MessageException("Nenhuma pessoa informada.");

        if (dto.Longitude == 0 && dto.Latitude == 0)
            throw new MessageException("Não foi possível detectar a sua localização.");

        RescueEntity entity = new()
        {
            RequestDateTime = DateTimeOffset.Now,
            UpdateDateTime = DateTimeOffset.Now,
            AdultsNumber = dto.AdultsNumber,
            ChildrenNumber = dto.ChildrenNumber,
            ElderlyNumber = dto.ElderlyNumber,
            DisabledNumber = dto.DisabledNumber,
            AnimalsNumber = dto.AnimalsNumber,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description,
            Status = RescueStatusEnum.Pending,
            RequestedBy = _userSession.UserId,
            ContactPhone = dto.ContactPhone ?? _userSession.Cellphone
        };

        await _serviceProvider.GetRequiredService<RescueRepository>().InsertOrUpdate(entity);

        return Response.Success(default(Object), "Solicitação de Resgate Registrada!");
    }
}