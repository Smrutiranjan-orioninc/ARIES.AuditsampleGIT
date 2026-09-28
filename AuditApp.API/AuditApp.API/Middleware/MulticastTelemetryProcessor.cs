using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace AuditApp.API.Middleware;

/// <summary>
/// Forwards a deep-clone of every telemetry item to the additional App Insights
/// connection strings (APPLICATIONINSIGHTS_CONNECTION_STRING2 … 5) so that all
/// five instances receive identical API telemetry.
/// </summary>
public sealed class MulticastTelemetryProcessor : ITelemetryProcessor, IDisposable
{
    private readonly ITelemetryProcessor _next;
    private readonly List<(TelemetryClient Client, TelemetryConfiguration Config, string? InstrumentationKey)> _extra = [];

    public MulticastTelemetryProcessor(ITelemetryProcessor next, IConfiguration configuration)
    {
        _next = next;

        foreach (var suffix in new[] { "2", "3", "4", "5" })
        {
            var cs = configuration[$"APPLICATIONINSIGHTS_CONNECTION_STRING{suffix}"];
            if (string.IsNullOrWhiteSpace(cs)) continue;

            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = cs;
            var instrumentationKey = GetInstrumentationKey(cs);
            _extra.Add((new TelemetryClient(config), config, instrumentationKey));
        }
    }

    public void Process(ITelemetry item)
    {
        // Send a copy to each additional App Insights instance
        foreach (var (client, _, instrumentationKey) in _extra)
        {
            var copy = item.DeepClone();

            // DeepClone preserves the original item's instrumentation key.
            // Override it so each forwarded item is routed to its target instance.
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                copy.Context.InstrumentationKey = instrumentationKey;
            }

            client.Track(copy);
        }

        // Continue normal pipeline → primary App Insights
        _next.Process(item);
    }

    public void Dispose()
    {
        foreach (var (_, config, _) in _extra)
            config.Dispose();
    }

    private static string? GetInstrumentationKey(string connectionString)
    {
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            if (part.StartsWith("InstrumentationKey=", StringComparison.OrdinalIgnoreCase))
            {
                return part["InstrumentationKey=".Length..];
            }
        }

        return null;
    }
}
