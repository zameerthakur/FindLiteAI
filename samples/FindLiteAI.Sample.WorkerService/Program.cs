using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Enums;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Extensions.DependencyInjection;

HostApplicationBuilder builder =
    Host.CreateApplicationBuilder(args);

string cacheDirectory =
    Path.Combine(
        Path.GetTempPath(),
        "FindLiteAI",
        "Models");

await ModelInstallService.InstallAsync(
    FindLiteAIModels.MiniLm,
    cacheDirectory,
    overwrite: false);

string modelDirectory =
    Path.Combine(
        cacheDirectory,
        FindLiteAIModels.MiniLm.Id);

builder.Services.AddFindLiteAI(options =>
{
    options.DatabasePath = "findliteai-worker-sample.db";
    options.ModelCacheDirectory = modelDirectory;
});

builder.Services.AddHostedService<Worker>();

IHost host =
    builder.Build();

await host.RunAsync();

/// <summary>
/// Demonstrates FindLiteAI usage inside a .NET Worker Service.
/// </summary>
internal sealed class Worker : BackgroundService
{
    private readonly ISemanticSearchEngine _searchEngine;
    private readonly ILogger<Worker> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="searchEngine">
    /// The FindLiteAI semantic search engine.
    /// </param>
    /// <param name="logger">
    /// The logger instance.
    /// </param>
    public Worker(
        ISemanticSearchEngine searchEngine,
        ILogger<Worker> logger)
    {
        _searchEngine = searchEngine;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "FindLiteAI Worker Service sample started.");

        await _searchEngine.AddRangeAsync(
            "worker_logs",
            [
                new SemanticDocument
                {
                    Id = "worker-log-1",
                    Text = "SFTP authentication failed for scheduled file transfer."
                },
                new SemanticDocument
                {
                    Id = "worker-log-2",
                    Text = "Database cleanup job completed successfully."
                },
                new SemanticDocument
                {
                    Id = "worker-log-3",
                    Text = "SMTP notification was sent to administrators."
                }
            ],
            stoppingToken);

        IReadOnlyList<SearchResult> results =
            await _searchEngine.SearchAsync(
                "worker_logs",
                "login issue",
                new SearchOptions
                {
                    SearchMode = SearchMode.Hybrid,
                    MaxResults = 5,
                    MinimumScore = 0.10
                },
                stoppingToken);

        foreach (SearchResult result in results)
        {
            _logger.LogInformation(
                "Result {Rank}: {DocumentId} | Score: {Score} | Text: {Text}",
                result.Rank,
                result.Document.Id,
                result.Score,
                result.Document.Text);
        }

        _logger.LogInformation(
            "FindLiteAI Worker Service sample completed.");
    }
}
