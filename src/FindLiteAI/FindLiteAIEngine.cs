using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;
using FindLiteAI.Internal.Validation;
using Microsoft.Extensions.Logging;

namespace FindLiteAI;

/// <summary>
/// Provides the primary implementation of the FindLiteAI semantic search engine.
/// </summary>
public sealed class FindLiteAIEngine : ISemanticSearchEngine
{
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly ISemanticStore _semanticStore;
    private readonly ILogger<FindLiteAIEngine> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FindLiteAIEngine"/> class.
    /// </summary>
    /// <param name="embeddingProvider">The embedding provider implementation.</param>
    /// <param name="semanticStore">The semantic document store implementation.</param>
    /// <param name="logger">The logger instance.</param>
    public FindLiteAIEngine(
        IEmbeddingProvider embeddingProvider,
        ISemanticStore semanticStore,
        ILogger<FindLiteAIEngine> logger)
    {
        _embeddingProvider = embeddingProvider;
        _semanticStore = semanticStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task AddAsync(
        string collection,
        SemanticDocument document,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchEngineValidator.ValidateCollection(collection);
            SearchEngineValidator.ValidateDocument(document);

            _logger.LogDebug(
                "Indexing document '{DocumentId}' in collection '{Collection}'.",
                document.Id,
                collection);

            IReadOnlyList<float> embedding =
                await _embeddingProvider.GenerateEmbeddingAsync(
                    document.Text,
                    cancellationToken);

            await _semanticStore.UpsertAsync(
                collection,
                document,
                embedding,
                cancellationToken);

            _logger.LogInformation(
                "Indexed document '{DocumentId}' in collection '{Collection}'.",
                document.Id,
                collection);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to index document '{DocumentId}' in collection '{Collection}'.",
                document.Id,
                collection);

            throw new SearchException(
                $"Failed to index document '{document.Id}' in collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchEngineValidator.ValidateCollection(collection);
            ArgumentNullException.ThrowIfNull(documents);

            foreach (SemanticDocument document in documents)
            {
                SearchEngineValidator.ValidateDocument(document);
            }

            _logger.LogInformation(
                "Indexing {DocumentCount} documents in collection '{Collection}'.",
                documents.Count,
                collection);

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

            _logger.LogInformation(
                "Indexed {DocumentCount} documents in collection '{Collection}'.",
                documents.Count,
                collection);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to index documents in collection '{Collection}'.",
                collection);

            throw new SearchException(
                $"Failed to index documents in collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        string collection,
        string query,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchEngineValidator.ValidateCollection(collection);
            SearchEngineValidator.ValidateQuery(query);

            SearchOptions resolvedOptions = options ?? new SearchOptions();

            SearchEngineValidator.ValidateSearchOptions(resolvedOptions);

            _logger.LogDebug(
                "Searching collection '{Collection}' using mode '{SearchMode}'.",
                collection,
                resolvedOptions.SearchMode);

            IReadOnlyList<float> queryEmbedding =
                await _embeddingProvider.GenerateEmbeddingAsync(
                    query,
                    cancellationToken);

            IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> indexedItems =
                await _semanticStore.GetAllAsync(
                    collection,
                    cancellationToken);

            IReadOnlyList<SearchResult> results =
                Internal.Services.SearchRankingService.Rank(
                    indexedItems,
                    query,
                    queryEmbedding,
                    resolvedOptions);

            _logger.LogInformation(
                "Search completed for collection '{Collection}'. Returned {ResultCount} results.",
                collection,
                results.Count);

            return results;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Search failed for collection '{Collection}'.",
                collection);

            throw new SearchException(
                $"Search failed for collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SearchResult>> FindSimilarAsync(
        string collection,
        string documentId,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchEngineValidator.ValidateCollection(collection);

            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentException(
                    "Document identifier cannot be null or empty.",
                    nameof(documentId));
            }

            SearchOptions resolvedOptions = options ?? new SearchOptions();

            SearchEngineValidator.ValidateSearchOptions(resolvedOptions);

            _logger.LogDebug(
                "Finding documents similar to '{DocumentId}' in collection '{Collection}'.",
                documentId,
                collection);

            (SemanticDocument Document, IReadOnlyList<float> Embedding)? sourceItem =
                await _semanticStore.GetByIdAsync(
                    collection,
                    documentId,
                    cancellationToken);

            if (sourceItem is null)
            {
                _logger.LogWarning(
                    "Source document '{DocumentId}' was not found in collection '{Collection}'.",
                    documentId,
                    collection);

                return [];
            }

            IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> indexedItems =
                await _semanticStore.GetAllAsync(
                    collection,
                    cancellationToken);

            IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> candidateItems =
                indexedItems
                    .Where(item => !string.Equals(
                        item.Document.Id,
                        documentId,
                        StringComparison.Ordinal))
                    .ToList();

            IReadOnlyList<SearchResult> results =
                Internal.Services.SearchRankingService.Rank(
                    candidateItems,
                    sourceItem.Value.Document.Text,
                    sourceItem.Value.Embedding,
                    resolvedOptions);

            _logger.LogInformation(
                "Similar document search completed for '{DocumentId}' in collection '{Collection}'. Returned {ResultCount} results.",
                documentId,
                collection,
                results.Count);

            return results;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Similar document search failed for '{DocumentId}' in collection '{Collection}'.",
                documentId,
                collection);

            throw new SearchException(
                $"Similar document search failed for document '{documentId}' in collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchEngineValidator.ValidateCollection(collection);

            if (string.IsNullOrWhiteSpace(documentId))
            {
                throw new ArgumentException(
                    "Document identifier cannot be null or empty.",
                    nameof(documentId));
            }

            await _semanticStore.DeleteAsync(
                collection,
                documentId,
                cancellationToken);

            _logger.LogInformation(
                "Deleted document '{DocumentId}' from collection '{Collection}'.",
                documentId,
                collection);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to delete document '{DocumentId}' from collection '{Collection}'.",
                documentId,
                collection);

            throw new SearchException(
                $"Failed to delete document '{documentId}' from collection '{collection}'.",
                exception);
        }
    }
}
