using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Internal.Validation;

namespace FindLiteAI;

/// <summary>
/// Provides the primary implementation of the FindLiteAI semantic search engine.
/// </summary>
public sealed class FindLiteAIEngine : ISemanticSearchEngine
{
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly ISemanticStore _semanticStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindLiteAIEngine"/> class.
    /// </summary>
    /// <param name="embeddingProvider">
    /// The embedding provider implementation.
    /// </param>
    /// <param name="semanticStore">
    /// The semantic document store implementation.
    /// </param>
    public FindLiteAIEngine(
        IEmbeddingProvider embeddingProvider,
        ISemanticStore semanticStore)
    {
        _embeddingProvider = embeddingProvider;
        _semanticStore = semanticStore;
    }

    /// <inheritdoc />
    public async Task AddAsync(
        string collection,
        SemanticDocument document,
        CancellationToken cancellationToken = default)
    {
        SearchEngineValidator.ValidateCollection(collection);

        SearchEngineValidator.ValidateDocument(document);

        IReadOnlyList<float> embedding =
            await _embeddingProvider.GenerateEmbeddingAsync(
                document.Text,
                cancellationToken);

        await _semanticStore.UpsertAsync(
            collection,
            document,
            embedding,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        CancellationToken cancellationToken = default)
    {
        SearchEngineValidator.ValidateCollection(collection);

        ArgumentNullException.ThrowIfNull(documents);

        foreach (SemanticDocument document in documents)
        {
            SearchEngineValidator.ValidateDocument(document);
        }

        IReadOnlyList<string> texts =
            documents
                .Select(document => document.Text)
                .ToList();

        IReadOnlyList<IReadOnlyList<float>> embeddings =
            await _embeddingProvider.GenerateEmbeddingsAsync(
                texts,
                cancellationToken);

        await _semanticStore.UpsertRangeAsync(
            collection,
            documents,
            embeddings,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SearchResult>> SearchAsync(
        string collection,
        string query,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SearchResult>> FindSimilarAsync(
        string collection,
        string documentId,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        SearchEngineValidator.ValidateCollection(collection);

        if (string.IsNullOrWhiteSpace(documentId))
        {
            throw new ArgumentException(
                "Document identifier cannot be null or empty.",
                nameof(documentId));
        }

        return _semanticStore.DeleteAsync(
            collection,
            documentId,
            cancellationToken);
    }
}
