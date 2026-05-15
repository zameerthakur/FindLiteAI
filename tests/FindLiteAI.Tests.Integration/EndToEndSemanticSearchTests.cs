using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains end-to-end integration tests for the full FindLiteAI pipeline.
/// </summary>
public sealed class EndToEndSemanticSearchTests
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
    /// Verifies that documents can be indexed, persisted, embedded, and searched end-to-end.
    /// </summary>
    [Fact]
    public async Task SearchAsync_WhenUsingRealModelAndLiteDb_ShouldReturnRelevantResult()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-e2e-{Guid.NewGuid():N}.db");

        await using ServiceProvider provider =
            await CreateProviderAsync(databasePath);

        ISemanticSearchEngine engine =
            provider.GetRequiredService<ISemanticSearchEngine>();

        await engine.AddRangeAsync(
            "logs",
            [
                new SemanticDocument
                {
                    Id = "log-1",
                    Text = "SFTP authentication failed for remote user."
                },
                new SemanticDocument
                {
                    Id = "log-2",
                    Text = "SQL database timeout occurred while executing query."
                },
                new SemanticDocument
                {
                    Id = "log-3",
                    Text = "SMTP email relay accepted outgoing message."
                }
            ]);

        IReadOnlyList<SearchResult> results =
            await engine.SearchAsync(
                "logs",
                "login issue",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.10,
                    MaxResults = 3
                });

        results.Should().NotBeEmpty();

        results
            .Select(result => result.Document.Id)
            .Should()
            .Contain("log-1");
    }

    /// <summary>
    /// Verifies that persisted documents can be searched after engine recreation.
    /// </summary>
    [Fact]
    public async Task SearchAsync_WhenEngineIsRecreated_ShouldSearchPersistedDocuments()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-e2e-persist-{Guid.NewGuid():N}.db");

        await using (ServiceProvider firstProvider = await CreateProviderAsync(databasePath))
        {
            ISemanticSearchEngine firstEngine =
                firstProvider.GetRequiredService<ISemanticSearchEngine>();

            await firstEngine.AddAsync(
                "docs",
                new SemanticDocument
                {
                    Id = "doc-1",
                    Text = "SMTP server configuration for outgoing mail."
                });
        }

        await using ServiceProvider secondProvider =
            await CreateProviderAsync(databasePath);

        ISemanticSearchEngine secondEngine =
            secondProvider.GetRequiredService<ISemanticSearchEngine>();

        IReadOnlyList<SearchResult> results =
            await secondEngine.SearchAsync(
                "docs",
                "email settings",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MinimumScore = 0.10,
                    MaxResults = 5
                });

        results.Should().NotBeEmpty();

        results[0].Document.Id.Should().Be("doc-1");
    }

    /// <summary>
    /// Creates a service provider configured with the real ONNX provider and LiteDB store.
    /// </summary>
    /// <param name="databasePath">
    /// The LiteDB database path.
    /// </param>
    /// <returns>
    /// A configured service provider.
    /// </returns>
    private static async Task<ServiceProvider> CreateProviderAsync(
        string databasePath)
    {
        string cacheDirectory = CreateCacheDirectory();

        await ModelInstallService.InstallAsync(
            FindLiteAIModels.MiniLm,
            cacheDirectory,
            overwrite: false);

        string modelDirectory =
            Path.Combine(
                cacheDirectory,
                FindLiteAIModels.MiniLm.Id);

        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelCacheDirectory = modelDirectory;
        });

        return services.BuildServiceProvider();
    }
}
