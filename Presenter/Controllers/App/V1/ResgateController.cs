﻿using Microsoft.AspNetCore.Mvc;
using ResgateRS.Domain.Application.Services;
using ResgateRS.Presenter.Controllers.App.V1.DTOs;

namespace ResgateRS.Presenter.Controllers.App.V1;

[ApiController]
[Route("api/v{version:apiVersion}/Rescue")]
[ApiVersion("1.0")]
public class RescueController(ResgateService service, IServiceProvider serviceProvider) : BaseController<ResgateService, IServiceProvider>(service, serviceProvider)
{

    [HttpPost("Request")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ResponseDTO>> RequestRescue(RescueRequestDTO dto) =>
        await this.mainService.RequestRescue(dto, Request.Headers.Authorization.ToString());

    [HttpPost("Confirm")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ResponseDTO>> ConfirmRescue(RescueConfirmDTO dto) =>
        await this.mainService.ConfirmRescue(dto, Request.Headers.Authorization.ToString());

    [HttpGet("ListMyRescues")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<RescueCardDTO>>> ListMyRescues(int page, int size) =>
        await this.mainService.ListMyRescues(page, size, Request.Headers.Authorization.ToString());

    [HttpGet("ListPengingRescues")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<RescueCardDTO>>> ListPengingRescues(int page, int size) =>
        await this.mainService.ListPendingRescues(page, size);

    [HttpGet("Details")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<RescueDTO>> DetailRescue(Guid rescueId) =>
        await this.mainService.DetailRescue(rescueId);
}

