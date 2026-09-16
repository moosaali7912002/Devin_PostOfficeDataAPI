using PostOfficeApi.Models.Dtos;

namespace PostOfficeApi.Services;

public interface IPostOfficeDataService
{
    Task<PostOfficeDataCreateResponse> CreateAsync(PostOfficeDataRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PostOfficeDataResponse>> CreateManyAsync(IEnumerable<PostOfficeDataRequest> requests, CancellationToken cancellationToken = default);

    Task<PostOfficeDataResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PostOfficeDataResponse>> GetByTrackingNoAsync(string trackingNo, CancellationToken cancellationToken = default);
    

}
