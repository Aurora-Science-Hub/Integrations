using AuroraScienceHub.Integrations.Noaa.Dscovr.Extensions;
using BenchmarkDotNet.Attributes;

namespace AuroraScienceHub.Integrations.Benchmarks.Noaa.Dscovr;

/// <summary>
/// Benchmarks for <see cref="MagnetometerDataParser"/> (DSCOVR).
/// </summary>
[MemoryDiagnoser(false)]
public class DscovrMagnetometerDataParserBenchmark
{
    const string ResourceName = "AuroraScienceHub.Integrations.Benchmarks.Noaa.Dscovr.Samples.DscovrMagnetometerSample.json";

    private string _text = string.Empty;

    [GlobalSetup]
    public async Task Setup()
    {
        await using var stream = typeof(DscovrMagnetometerDataParserBenchmark).Assembly
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
