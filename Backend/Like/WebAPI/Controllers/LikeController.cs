using Application.Commands.CreateLike;
using Application.Commands.DeleteLike;
using Application.Queries.GetLike;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly IMediator _mediator;

    public LikeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateLike(CreateLikeDto command)
    {
        var result = await _mediator.Send(new CreateLikeCommand(Guid.Parse(User.FindFirst("sub")!.Value), command.TargetType, command.ContentId));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetLike(GetLikeDto query)
    {
        var result = await _mediator.Send(new GetLikeQuery(Guid.Parse(User.FindFirst("sub")!.Value), query.TargetType, query.ContentId));

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLike(Guid id)
    {
        var result = await _mediator.Send(new DeleteLikeCommand(id, Guid.Parse(User.FindFirst("sub")!.Value)));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}
