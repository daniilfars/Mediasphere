using Application.Commands.CreateLike;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

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
}
