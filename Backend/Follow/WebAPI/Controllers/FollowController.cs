using Application.Commands.CreateFollow;
using Application.Queries.GetFollowers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FollowController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/follow/{followingId}
    [Authorize]
    [HttpPost("{followingId}")]
    public async Task<ActionResult> CreateFollow(Guid followingId)
    {
        var result = await _mediator.Send(new CreateFollowCommand(Guid.Parse(User.FindFirst("sub")!.Value), followingId));
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet()]
    public async Task<ActionResult<GetFollowersResponse>> GetFollowers([FromQuery] GetFollowersQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
