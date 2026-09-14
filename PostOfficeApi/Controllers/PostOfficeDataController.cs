using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostOfficeApi.Authentication;
using PostOfficeApi.Models.Dtos;
using PostOfficeApi.Services;

namespace PostOfficeApi.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = SignatureAuthenticationDefaults.AuthenticationScheme)]
[Route("api/post-office-data")]
[Produces("application/json")]
public class PostOfficeDataController : ControllerBase
{
    private readonly IPostOfficeDataService _service;

    public PostOfficeDataController(IPostOfficeDataService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostOfficeDataResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostOfficeDataResponse>> Create([FromBody] PostOfficeDataRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IReadOnlyList<PostOfficeDataResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PostOfficeDataResponse>>> CreateMany([FromBody] List<PostOfficeDataRequest> requests, CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
        {
            return BadRequest("At least one record is required.");
        }

        var created = await _service.CreateManyAsync(requests, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostOfficeDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostOfficeDataResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var record = await _service.GetByIdAsync(id, cancellationToken);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpGet("by-tracking-no/{trackingNo}")]
    [ProducesResponseType(typeof(IReadOnlyList<PostOfficeDataResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PostOfficeDataResponse>>> GetByTrackingNo(string trackingNo, CancellationToken cancellationToken)
    {
        var records = await _service.GetByTrackingNoAsync(trackingNo, cancellationToken);
        return Ok(records);
    }
}
