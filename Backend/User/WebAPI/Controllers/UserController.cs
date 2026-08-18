using Application.Commands.UserCreated;
using Application.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/user
    [Authorize]
    [HttpPost()]
    public async Task<ActionResult<UserCreatedResponse>> UserCreated()
    {
        var result = await _mediator.Send(new UserCreatedCommand(
            Guid.Parse(User.FindFirst("sub")!.Value),
            User.FindFirst("preferred_username")!.Value
        ));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetUserById), new { userId = result.Value!.Id }, result.Value);
    }

    // GET: api/user
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetUserByIdResponse>> GetUserById(Guid userId)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(userId));

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}
