// Source code from: https://patriksvensson.se/posts/2017/11/using-embedded-resources-in-xunit-tests

using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace AuroraScienceHub.Integrations.UnitTests.Utils;

/// <summary>
/// Attribute to load test data from embedded resources.
/// </summary>
public sealed class EmbeddedResourceDataAttribute : DataAttribute
{
    private readonly string[] _args;

    public EmbeddedResourceDataAttribute(params string[] args)
    {
        _args = args;
    }

    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        var result = new object[_args.Length];
        for (var index = 0; index < _args.Length; index++)
        {
            result[index] = ReadManifestData(_args[index]);
        }
        return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>([new TheoryDataRow(result)]);
    }

    private static string ReadManifestData(string resourceName)
    {
        var assembly = typeof(EmbeddedResourceDataAttribute).GetTypeInfo().Assembly;
        resourceName = resourceName.Replace("/", ".");
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not load manifest resource stream: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public override bool SupportsDiscoveryEnumeration() => true;
}
