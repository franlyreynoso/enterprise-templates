using Microsoft.Extensions.Configuration;

namespace Enterprise.Ui.FeatureFlags;

public interface IFeatureFlagProvider { ValueTask<bool> IsEnabledAsync(string flag, CancellationToken ct = default); }

public sealed class ConfigFeatureFlags(IConfiguration cfg) : IFeatureFlagProvider
{
    public ValueTask<bool> IsEnabledAsync(string flag, CancellationToken ct = default) =>
        new((cfg[$"FeatureFlags:{flag}"] ?? "false").Equals("true", StringComparison.OrdinalIgnoreCase));
}
