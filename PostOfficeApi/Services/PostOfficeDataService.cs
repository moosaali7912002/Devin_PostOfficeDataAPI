using Microsoft.EntityFrameworkCore;
using PostOfficeApi.Data;
using PostOfficeApi.Models;
using PostOfficeApi.Models.Dtos;

namespace PostOfficeApi.Services;

public class PostOfficeDataService : IPostOfficeDataService
{
    private readonly PostOfficeDbContext _dbContext;
    private readonly ILogger<PostOfficeDataService> _logger;

    public PostOfficeDataService(PostOfficeDbContext dbContext, ILogger<PostOfficeDataService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PostOfficeDataCreateResponse> CreateAsync(PostOfficeDataRequest request, CancellationToken cancellationToken = default)
    {
        var receivedAt = DateTime.UtcNow.AddHours(5);
        try
        {
            var entity = MapToEntity(request);
            // Set the date/time when the payload was received by the API
            entity.IncomigPayloadReceivedDateTime = receivedAt;

            _dbContext.PostOfficeData.Add(entity);
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            if (affectedRows < 1)
            {
                _logger.LogWarning("Post office data was not saved for tracking number {TrackingNo}", entity.PO_TrackingNo);

                return new PostOfficeDataCreateResponse
                {
                    Success = false,
                    Message = "Data was not recorded in the database. No records were affected.",
                    TrackingNo = entity.PO_TrackingNo,
                    RecordId = 0,
                    RecordedAt = receivedAt
                };
            }
            else
            {

                _logger.LogInformation("Stored post office record {Id} for tracking number {TrackingNo}", entity.Id, entity.PO_TrackingNo);

                //MapToResponse(entity);
                return new PostOfficeDataCreateResponse
                {
                    Success = true,
                    Message = "Data successfully recorded.",
                    TrackingNo = entity.PO_TrackingNo,
                    RecordId = entity.Id,
                    RecordedAt = receivedAt
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Failed to store post office data for tracking number {TrackingNo}",request.tracking_no);

            return new PostOfficeDataCreateResponse
            {
                Success = false,
                Message = $"Data could not be recorded. Reason: {ex.Message}",
                TrackingNo = request.tracking_no,
                RecordId = 0,
                RecordedAt = receivedAt
            };
        }
    }


    public async Task<IReadOnlyList<PostOfficeDataResponse>> CreateManyAsync(IEnumerable<PostOfficeDataRequest> requests, CancellationToken cancellationToken = default)
    {
        var entities = requests.Select(MapToEntity).ToList();

        _dbContext.PostOfficeData.AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stored {Count} post office records", entities.Count);

        return entities.Select(MapToResponse).ToList();
    }

    public async Task<PostOfficeDataResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.PostOfficeData
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<IReadOnlyList<PostOfficeDataResponse>> GetByTrackingNoAsync(string trackingNo, CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.PostOfficeData
            .AsNoTracking()
            .Where(e => e.PO_TrackingNo == trackingNo)
            .OrderByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToResponse).ToList();
    }

    private static PostOfficeData MapToEntity(PostOfficeDataRequest request) => new()
    {
        PO_CustomerName = request.customer_name,
        PO_TrackingNo = request.tracking_no,
        PO_MobileNo = request.mobile_no,
        PO_EmailAddress = request.email_address,
        PO_Weight = request.weight,
        PO_CurrentDestination = request.current_destination,
        PO_CurrentLocation = request.current_location,
        PO_CreatedAt = request.created_at,
        PO_MplUpdatedAt = request.updated_at,
        PO_OriginCountryCode = request.origin_country,
        PO_DestinationCountryCode = request.destination_country,
        PO_ShippingAddress = request.shipping_address,
        PO_ItemsDescription = request.items,
        PO_Pieces = request.pieces,
        PO_Value = request.value,
        PO_PackageNumber = request.package_number,
        PO_ServiceType = request.ServiceType,
        PO_PackageLastStatus = request.PackageLastStatus
    };

    private static PostOfficeDataResponse MapToResponse(PostOfficeData entity) => new()
    {
        Id = entity.Id,
        PO_CustomerName = entity.PO_CustomerName,
        PO_TrackingNo = entity.PO_TrackingNo,
        PO_MobileNo = entity.PO_MobileNo,
        PO_EmailAddress = entity.PO_EmailAddress,
        PO_Weight = entity.PO_Weight,
        PO_CurrentDestination = entity.PO_CurrentDestination,
        PO_CurrentLocation = entity.PO_CurrentLocation,
        PO_CreatedAt = entity.PO_CreatedAt,
        PO_MplUpdatedAt = entity.PO_MplUpdatedAt,
        PO_OriginCountryCode = entity.PO_OriginCountryCode,
        PO_DestinationCountryCode = entity.PO_DestinationCountryCode,
        PO_ShippingAddress = entity.PO_ShippingAddress,
        PO_ItemsDescription = entity.PO_ItemsDescription,
        PO_Pieces = entity.PO_Pieces,
        PO_Value = entity.PO_Value,
        PO_PackageNumber = entity.PO_PackageNumber,
        PO_ServiceType = entity.PO_ServiceType,
        PO_PackageLastStatus = entity.PO_PackageLastStatus
    };

}
