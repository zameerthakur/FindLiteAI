using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains end-to-end integration tests for the full FindLiteAI pipeline.
/// </summary>
public sealed class EndToEndSemanticSearchTests
{
    private const string ModelPath =
        @"D:\AIModels\FindLiteAI\all-MiniLM-L6-v2\model.onnx";

    /// <summary>
    /// Verifies that documents can be indexed, persisted, embedded, and searched end-to-end.
    /// </summary>
    [Fact]
    public async Task SearchAsync_WhenUsingRealModelAndLiteDb_ShouldReturnRelevantResult()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-e2e-{Guid.NewGuid():N}.db");

        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelPath = ModelPath;
        });

        using ServiceProvider provider = services.BuildServiceProvider();

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

        await using (ServiceProvider firstProvider = CreateProvider(databasePath))
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

        await using ServiceProvider secondProvider = CreateProvider(databasePath);

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
    private static ServiceProvider CreateProvider(
        string databasePath)
    {
        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelPath = ModelPath;
        });

        return services.BuildServiceProvider();
    }
}
