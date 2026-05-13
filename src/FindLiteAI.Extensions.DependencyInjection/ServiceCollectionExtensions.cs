using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Options;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Storage.LiteDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FindLiteAI.Extensions.DependencyInjection;

/// <summary>
/// Provides dependency injection registration extensions for FindLiteAI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers FindLiteAI services using the provided configuration.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <param name="configure">
    /// The FindLiteAI configuration delegate.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection AddFindLiteAI(
        this IServiceCollection services,
        Action<FindLiteAIOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        FindLiteAIOptions options = new();

        configure(options);

        services.AddSingleton(options);

        services.AddSingleton(provider =>
        {
            ILogger<OnnxEmbeddingProvider> logger =
                provider.GetRequiredService<ILogger<OnnxEmbeddingProvider>>();

            if (!string.IsNullOrWhiteSpace(options.ModelCacheDirectory))
            {
                return OnnxEmbeddingProviderFactory.FromModelPackage(
                    options.ModelCacheDirectory,
                    logger);
            }

            return new OnnxEmbeddingProvider(
                new OnnxEmbeddingProviderOptions
                {
                    ModelPath = options.ModelPath
                        ?? throw new InvalidOperationException(
                            "ModelPath or ModelCacheDirectory must be configured for FindLiteAI."),
                    VocabularyPath = ResolveVocabularyPath(options),
                    MaxTokenLength = 256,
                    WarmupOnLoad = true
                },
                logger);
        });

        services.AddSingleton<IEmbeddingProvider>(provider =>
            provider.GetRequiredService<OnnxEmbeddingProvider>());

        services.AddSingleton<ISemanticStore>(provider =>
            new LiteDbSemanticStore(
                new LiteDbOptions
                {
                    DatabasePath = options.DatabasePath
                },
                provider.GetRequiredService<ILogger<LiteDbSemanticStore>>()));

        services.AddLogging();

        services.AddSingleton<ISemanticSearchEngine, FindLiteAIEngine>();

        return services;
    }

    private static string ResolveVocabularyPath(
        FindLiteAIOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ModelPath))
        {
            throw new InvalidOperationException(
                "ModelPath must be configured before resolving vocabulary path.");
        }

        string? directory =
            Path.GetDirectoryName(options.ModelPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "ModelPath must include a valid directory.");
        }

        return Path.Combine(
            directory,
            "vocab.txt");
    }
}
