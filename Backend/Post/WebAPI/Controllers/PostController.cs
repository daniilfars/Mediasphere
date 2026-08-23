using Application.Commands.CreatePost;
using Application.Commands.DeletePost;
using Application.Commands.UpdatePost;
using Application.Commands.UploadPostImage;
using Application.Queries.GetPostById;
using Application.Queries.GetPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/post
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreatePostResponse>> CreatePost([FromForm]string content, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Файл не выбран или пустой");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Размер файла не должен превышать 5 МБ");

        await using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new CreatePostCommand(
            Guid.Parse(User.FindFirst("sub")!.Value), User.FindFirst("preferred_username")!.Value, content, stream, file.ContentType
        ));

        if (result.IsFailure)
            return BadRequest(result.Error!);

        return CreatedAtAction(nameof(GetPostById), new { id = result.Value!.Id }, result.Value);
    }

    // GET: api/post/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPostByIdResponse>> GetPostById(Guid id)
    {
        var result = await _mediator.Send(new GetPostByIdQuery(id));

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    // GET: api/post
    [HttpGet()]
    public async Task<ActionResult<GetPostByIdResponse>> GetPosts([FromQuery] GetPostsQuery query)
    {
        var result = await _mediator.Send(query);

        return Ok(result.Value);
    }

    // PUT: api/post
    [Authorize]
    [HttpPut()]
    public async Task<ActionResult<GetPostByIdResponse>> UpdatePost(UpdatePostRequest request)
    {
        var result = await _mediator.Send(new UpdatePostCommand(request.Id, Guid.Parse(User.FindFirst("sub")!.Value), request.Content));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    // POST: api/post/{id}/upload-image
    [Authorize]
    [HttpPost("{id}/upload-image")]
    public async Task<ActionResult<UploadPostImageResponse>> UploadPostImage(Guid id, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Файл не выбран или пустой");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Размер файла не должен превышать 5 МБ");

        await using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new UploadPostImageCommand(id, Guid.Parse(User.FindFirst("sub")!.Value), stream, file.ContentType));

        if (result.IsFailure)
            return BadRequest(result.Error!);

        return Ok(result.Value);
    }

    // DELETE: api/post/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var result = await _mediator.Send(new DeletePostCommand(id, Guid.Parse(User.FindFirst("sub")!.Value)));
        if (result.IsFailure)
            return BadRequest(result.Error!);

        return NoContent();
    }
}
