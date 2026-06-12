using System;
using Hangfire;
using Hangfire.MemoryStorage;

namespace AvaloniaTestDemo.Services;

public sealed class HangfireService : IDisposable
{
    private readonly BackgroundJobServer _server;

    public HangfireService()
    {
        GlobalConfiguration.Configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage();

        _server = new BackgroundJobServer();
    }

    public void Dispose() => _server.Dispose();
}
