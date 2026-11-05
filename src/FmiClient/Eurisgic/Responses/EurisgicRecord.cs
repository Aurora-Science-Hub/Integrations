namespace AuroraScienceHub.Integrations.FmiClient.Eurisgic.Responses;

/// <summary>
/// FMI EURISGIC real-time electrojet indices
/// </summary>
/// <param name="DateTime">Record timestamp in UTC; a provider timestamp with second 60 is normalized to the next minute</param>
/// <param name="Iu">IU index in nT, or null when the provider publishes NaN</param>
/// <param name="Il">IL index in nT, or null when the provider publishes NaN</param>
/// <param name="StationCount">Number of magnetometer stations used for the record</param>
public sealed record EurisgicRecord(DateTime DateTime, float? Iu, float? Il, int StationCount);
