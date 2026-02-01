using AuroraScienceHub.Integrations.Noaa.Ace.Extensions;
using BenchmarkDotNet.Attributes;

namespace AuroraScienceHub.Integrations.Benchmarks.Noaa.Ace;

/// <summary>
/// Benchmarks for <see cref="SolarWindPlasmaDataParser"/> (ACE).
/// </summary>
[MemoryDiagnoser(false)]
public class AceSolarWindPlasmaDataParserBenchmark
{
    const string ResourceName = "AuroraScienceHub.Integrations.Benchmarks.Noaa.Ace.Samples.AceSwepamSample.txt";

    private string _text = string.Empty;

    [GlobalSetup]
    public async Task Setup()
    {
        await using var stream = typeof(AceMagnetometerDataParserBenchmark).Assembly
                                     .GetManifestResourceStream(ResourceName)
                                 ?? throw new FileNotFoundException($"Resource '{ResourceName}' not found.");
        using var reader = new StreamReader(stream);
        _text = await reader.ReadToEndAsync();
    }

    [Benchmark]
    public void Parse()
    {
        SolarWindPlasmaDataParser.Parse(_text);
    }
}
