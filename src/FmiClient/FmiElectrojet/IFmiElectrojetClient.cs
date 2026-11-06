using AuroraScienceHub.Integrations.FmiClient.FmiElectrojet.Responses;

namespace AuroraScienceHub.Integrations.FmiClient.FmiElectrojet;

/// <summary>
/// FMI real-time electrojet (IU and IL) indices client
/// </summary>
/// <remarks>
/// The preliminary IU and IL values are retrieved together from one file covering the latest 24 hours.
/// FMI does not manually check the data quality
/// </remarks>
public interface IFmiElectrojetClient
{
    /// <summary>
    /// Gets the latest 1-minute IU and IL indices in source order
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Published records, or an empty list for an empty response body</returns>
    Task<IReadOnlyList<FmiElectrojetRecord>> GetIndicesAsync(CancellationToken cancellationToken);
}
