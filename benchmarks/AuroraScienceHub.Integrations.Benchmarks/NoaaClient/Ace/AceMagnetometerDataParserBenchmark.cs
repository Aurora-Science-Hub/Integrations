using AuroraScienceHub.Integrations.NoaaClient.Ace.Extensions;
using BenchmarkDotNet.Attributes;

namespace AuroraScienceHub.Integrations.Benchmarks.NoaaClient.Ace;

/// <summary>
/// Benchmarks for <see cref="MagnetometerDataParser"/> (ACE).
/// </summary>
[MemoryDiagnoser(false)]
public class AceMagnetometerDataParserBenchmark
{
    const string ResourceName = "AuroraScienceHub.Integrations.Benchmarks.NoaaClient.Ace.Samples.AceMagnetometerSample.txt";

    private string _text = string.Empty;

    [GlobalSetup]
    public async Task Setup()
    {
        await using var stream = typeof(IBenchmarksMarker).Assembly
                                     .GetManifestResourceStream(ResourceName)
                                 ?? throw new FileNotFoundException($"Resource '{ResourceName}' not found.");
        using var reader = new StreamReader(stream);
        _text = await reader.ReadToEndAsync();
    }

    [Benchmark]
    public void Parse()
    {
        MagnetometerDataParser.Parse(_text);
    }
}
