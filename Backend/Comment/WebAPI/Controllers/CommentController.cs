using Application.Commands.CreateComment;
using Application.Queries.GetPostComments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/comment/
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateComment(CreateCommentDto command)
    {
        var result = await _mediator.Send(new CreateCommentCommand(Guid.Parse(User.FindFirst("sub")!.Value), command.PostId, command.Content));
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
        //return CreatedAtAction(nameof(GetCommentById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<GetPostCommentsResponse>> GetPostComments([FromQuery]GetPostCommentsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result.Value);
    }
}