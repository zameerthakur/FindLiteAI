using FindLiteAI.Core.Abstractions;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for FindLiteAI dependency injection registration.
/// </summary>
public sealed class FindLiteAIDependencyInjectionTests
{
    private static string CreateCacheDirectory()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "FindLiteAI",
            "Tests",
            Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Verifies that FindLiteAI services can be registered and resolved through dependency injection.
    /// </summary>
    [Fact]
    public async Task AddFindLiteAI_WhenConfigured_ShouldResolveSemanticSearchEngine()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-di-{Guid.NewGuid():N}.db");

        string modelDirectory =
            await InstallMiniLmAsync();

        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelCacheDirectory = modelDirectory;
        });

        using ServiceProvider provider = services.BuildServiceProvider();

        ISemanticSearchEngine engine =
            provider.GetRequiredService<ISemanticSearchEngine>();

        engine.Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that FindLiteAI can resolve services using an extracted model package directory.
    /// </summary>
    [Fact]
    public async Task AddFindLiteAI_WhenModelPackageDirectoryIsConfigured_ShouldResolveSemanticSearchEngine()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-di-package-{Guid.NewGuid():N}.db");

        string modelDirectory =
            await InstallMiniLmAsync();

        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelCacheDirectory = modelDirectory;
        });

        using ServiceProvider provider = services.BuildServiceProvider();

        ISemanticSearchEngine engine =
            provider.GetRequiredService<ISemanticSearchEngine>();

        engine.Should().NotBeNull();
    }

    /// <summary>
    /// Installs the MiniLM model package into the test cache and returns the model directory.
    /// </summary>
    /// <returns>
    /// The installed MiniLM model directory.
    /// </returns>
    private static async Task<string> InstallMiniLmAsync()
    {
        string cacheDirectory = CreateCacheDirectory();

        await ModelInstallService.InstallAsync(
            FindLiteAIModels.MiniLm,
            cacheDirectory,
            overwrite: false);

        return Path.Combine(
            cacheDirectory,
            FindLiteAIModels.MiniLm.Id);
    }
}
