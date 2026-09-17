using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostOfficeApi.Authentication;
using PostOfficeApi.Models.Dtos;
using PostOfficeApi.Services;

namespace PostOfficeApi.Controllers;

[ApiController]
//[Authorize(AuthenticationSchemes = SignatureAuthenticationDefaults.AuthenticationScheme)]
[Route("api/post-office-data")]
[Produces("application/json")]
public class PostOfficeDataController : ControllerBase
{
    private readonly IPostOfficeDataService _service;
    private readonly ILogger<PostOfficeDataController> _logger;

    public PostOfficeDataController(IPostOfficeDataService service, ILogger<PostOfficeDataController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostOfficeDataCreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PostOfficeDataCreateResponse>> Create([FromBody] PostOfficeDataRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.RecordId }, created);
        }
        catch (OperationCanceledException)
        {
            // Let the client know the request timed out / was cancelled
            _logger.LogInformation("Create request cancelled by client.");
            return Problem(title: "Request cancelled", statusCode: StatusCodes.Status499ClientClosedRequest);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database failure while creating post office data for tracking number {TrackingNo}", request?.tracking_no);
            var pd = new ProblemDetails
            {
                Title = "Database error",
                Detail = "A database error occurred while recording the data. Please retry.",
                Status = StatusCodes.Status500InternalServerError
            };
            return StatusCode(StatusCodes.Status500InternalServerError, pd);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while creating post office data for tracking number {TrackingNo}", request?.tracking_no);
            var pd = new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while processing the request.",
                Status = StatusCodes.Status500InternalServerError
            };
            return StatusCode(StatusCodes.Status500InternalServerError, pd);
        }
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
