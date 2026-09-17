using System.Globalization;
using System.Text.Json;
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
        if (request is null) throw new ArgumentNullException(nameof(request));
        cancellationToken.ThrowIfCancellationRequested();

        var receivedAt = DateTime.UtcNow;

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

        _logger.LogInformation("Stored post office record {Id} for tracking number {TrackingNo}", entity.Id, entity.PO_TrackingNo);

        return new PostOfficeDataCreateResponse
        {
            Success = true,
            Message = "Data successfully recorded.",
            TrackingNo = entity.PO_TrackingNo,
            RecordId = entity.Id,
            RecordedAt = receivedAt
        };
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

    private static PostOfficeData MapToEntity(PostOfficeDataRequest request)
    {
        var entity = new PostOfficeData
        {
            PO_CustomerName = request.customer_name,
            PO_TrackingNo = request.tracking_no,
            PO_MobileNo = request.mobile_no,
            PO_EmailAddress = request.email_address,
            PO_Weight = request.weight,
            PO_MailClass = request.mail_class,
            PO_MailSubClass = request.mail_sub_class,
            PO_CurrentDestination = request.current_destination,
            PO_CurrentLocation = request.current_location,
            //PO_CreatedAt = request.created_at,
            //PO_MplUpdatedAt = request.updated_at,
            PO_OriginCountryCode = request.origin_country,
            PO_DestinationCountryCode = request.destination_country,
            PO_ShippingAddress = request.shipping_address,
            PO_ItemsDescription = request.items,
            PO_Pieces = request.pieces,
            PO_Value = request.value,
            PO_PackageNumber = request.package_number,
            PO_ScanTimeZone = request.scan_time_zone,
            PO_PackageLastStatus = request.PackageLastStatus
        };

        // Parse incoming created_at/updated_at strings. Expecting format "dd-MM-yyyy HH:mm:ss".
        entity.PO_CreatedAt = ParseIncomingDateTimeString(request.created_at, request.scan_time_zone);
        entity.PO_MplUpdatedAt = ParseIncomingDateTimeString(request.updated_at, request.scan_time_zone);

        // Map events into child entities
        if (request.postOfficeDataRequestEvents != null && request.postOfficeDataRequestEvents.Count > 0)
        {
            entity.postOfficeDataEvents = request.postOfficeDataRequestEvents
                .Select(e => new PostOfficeDataEvent
                {
                    PO_EventDetails = e.details,
                    PO_EventCreatedAt = ParseIncomingDateTimeString(e.created_at, request.scan_time_zone)
                })
                .ToList();
        }
        return entity;
    }

    private static PostOfficeDataResponse MapToResponse(PostOfficeData entity) => new()
    {

        Id = entity.Id,
        PO_CustomerName = entity.PO_CustomerName,
        PO_TrackingNo = entity.PO_TrackingNo,
        PO_MobileNo = entity.PO_MobileNo,
        PO_EmailAddress = entity.PO_EmailAddress,
        PO_Weight = entity.PO_Weight,
        PO_MailClass = entity.PO_MailClass,
        PO_MailSubClass = entity.PO_MailSubClass,
        PO_CurrentDestination = entity.PO_CurrentDestination,
        PO_CurrentLocation = entity.PO_CurrentLocation,
        //PO_CreatedAt = entity.PO_CreatedAt,
        //PO_MplUpdatedAt = entity.PO_MplUpdatedAt,
        PO_OriginCountryCode = entity.PO_OriginCountryCode,
        PO_DestinationCountryCode = entity.PO_DestinationCountryCode,
        PO_ShippingAddress = entity.PO_ShippingAddress,
        PO_ItemsDescription = entity.PO_ItemsDescription,
        PO_Pieces = entity.PO_Pieces,
        PO_Value = entity.PO_Value,
        PO_PackageNumber = entity.PO_PackageNumber,
        PO_ScanTimeZone = entity.PO_ScanTimeZone,
        PO_PackageLastStatus = entity.PO_PackageLastStatus,
        
    };

    private static DateTime? ParseIncomingDateTime(DateTime? dateTime, string? timeZone)
    {
        // If the incoming value is null, return null
        if (dateTime == null)
            return null;

        // If a timezone is specified, try to convert to UTC using the timezone
        if (!string.IsNullOrWhiteSpace(timeZone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc(dateTime.Value, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback to UTC if timezone is invalid
                return dateTime.Value.ToUniversalTime();
            }
            catch (InvalidTimeZoneException)
            {
                return dateTime.Value.ToUniversalTime();
            }
        }

        // If no timezone is specified, assume the DateTime is already UTC or local and convert to UTC
        return dateTime.Value.ToUniversalTime();
    }

    // Helper to parse string to DateTime?
    private static DateTime? ParseIncomingDateTimeString(string? dateTimeString, string? timeZone)
    {
        if (string.IsNullOrWhiteSpace(dateTimeString))
            return null;

        // Try to parse the string to DateTime
        if (!DateTime.TryParseExact(dateTimeString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
            return null;

        // Use the existing logic to convert to UTC
        return ParseIncomingDateTime(parsedDateTime, timeZone);
    }
}
