using Application.Queries.GetUser;
using MediatR;
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
