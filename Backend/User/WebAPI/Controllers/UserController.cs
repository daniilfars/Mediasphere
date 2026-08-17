using Application.Commands.UserCreated;
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
    public async Task<ActionResult<UserCreatedResponse>> Create()
    {
        var result = await _mediator.Send(new UserCreatedCommand(
            Guid.Parse(User.FindFirst("sub")!.Value),
            User.FindFirst("preferred_username")!.Value
        ));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
