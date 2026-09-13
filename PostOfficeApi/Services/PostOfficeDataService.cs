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

    public async Task<PostOfficeDataResponse> CreateAsync(PostOfficeDataRequest request, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(request);

        _dbContext.PostOfficeData.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Stored post office record {Id} for tracking number {TrackingNo}", entity.Id, entity.PO_TrackingNo);

        return MapToResponse(entity);
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
        PO_CustomerName = request.PO_CustomerName,
        PO_TrackingNo = request.PO_TrackingNo,
        PO_MobileNo = request.PO_MobileNo,
        PO_EmailAddress = request.PO_EmailAddress,
        PO_Weight = request.PO_Weight,
        PO_CurrentDestination = request.PO_CurrentDestination,
        PO_CurrentLocation = request.PO_CurrentLocation,
        PO_CreatedAt = request.PO_CreatedAt ?? DateTime.UtcNow,
        PO_MplUpdatedAt = request.PO_MplUpdatedAt,
        PO_OriginCountryCode = request.PO_OriginCountryCode,
        PO_DestinationCountryCode = request.PO_DestinationCountryCode,
        PO_ShippingAddress = request.PO_ShippingAddress,
        PO_ItemsDescription = request.PO_ItemsDescription,
        PO_Pieces = request.PO_Pieces,
        PO_Value = request.PO_Value,
        PO_PackageNumber = request.PO_PackageNumber,
        PO_ServiceType = request.PO_ServiceType,
        PO_PackageLastStatus = request.PO_PackageLastStatus
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
