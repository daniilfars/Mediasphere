using Application.Commands.CreateFollow;
using Application.Commands.DeleteFollow;
using Application.Queries.GetFollowers;
using Application.Queries.GetFollowings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    // GET: api/follow/followers?UserId={}&Page=1&PageSize=10
    [HttpGet("followers")]
    public async Task<ActionResult<GetFollowersResponse>> GetFollowers([FromQuery] GetFollowersQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // GET: api/follow/followings?UserId{}&Page=1&PageSize=10
    [HttpGet("followings")]
    public async Task<ActionResult<GetFollowingsResponse>> GetFollowings([FromQuery] GetFollowingsQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // DELETE: api/follow
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteFollow(DeleteFollowCommand command)
    {
        var result = await _mediator.Send(command);
        if(result.IsFailure)
            return NotFound(result.Error);

        return NoContent();
    }
}