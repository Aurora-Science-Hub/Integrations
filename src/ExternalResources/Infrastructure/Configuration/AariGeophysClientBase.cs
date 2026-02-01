using AuroraScienceHub.Framework.Http;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure.Configuration;

internal abstract class AariGeophysClientBase
{
    private readonly Uri _baseUri;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Relative path to data generating scripts
    /// </summary>
    protected abstract string BasePath { get; }

    /// <summary> .ctor </summary>
    protected AariGeophysClientBase(HttpClient httpClient, IOptions<AariGeophysClientOptions> options)
    {
        _baseUri = options.Value.RequiredServerUrl;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Build full URL to the file
    /// </summary>
    /// <param name="dataUrl"> File name </param>
    protected Uri GetPathToData(string dataUrl) => UrlBuilder.From(_baseUri, BasePath + dataUrl).Build();

    /// <summary>
    /// Send request
    /// </summary>
    /// <param name="request"> Request </param>
    /// <param name="cancellationToken"> Cancellation token </param>
    protected async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) => await _httpClient.SendAsync(request, cancellationToken);
}
