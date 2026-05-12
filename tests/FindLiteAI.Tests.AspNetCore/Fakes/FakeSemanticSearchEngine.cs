using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;

namespace FindLiteAI.Tests.AspNetCore.Fakes;

/// <summary>
/// Provides a fake semantic search engine for ASP.NET Core endpoint tests.
/// </summary>
internal sealed class FakeSemanticSearchEngine : ISemanticSearchEngine
{
    private readonly Dictionary<string, Dictionary<string, SemanticDocument>> _collections = [];

    /// <inheritdoc />
    public Task AddAsync(
        string collection,
        SemanticDocument document,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, SemanticDocument> documents =
            GetOrCreateCollection(collection);

        documents[document.Id] = document;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, SemanticDocument> targetCollection =
            GetOrCreateCollection(collection);

        foreach (SemanticDocument document in documents)
        {
            targetCollection[document.Id] = document;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SearchResult>> SearchAsync(
        string collection,
        string query,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (!_collections.TryGetValue(collection, out Dictionary<string, SemanticDocument>? documents))
        {
            return Task.FromResult<IReadOnlyList<SearchResult>>([]);
        }

        IReadOnlyList<SearchResult> results =
            documents
                .Values
                .Select((document, index) =>
                    new SearchResult
                    {
                        Document = document,
                        Score = 1,
                        Rank = index
                    })
                .ToList();

        return Task.FromResult(results);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SearchResult>> FindSimilarAsync(
        string collection,
        string documentId,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SearchResult>>([]);
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (_collections.TryGetValue(collection, out Dictionary<string, SemanticDocument>? documents))
        {
            documents.Remove(documentId);
        }

        return Task.CompletedTask;
    }

    private Dictionary<string, SemanticDocument> GetOrCreateCollection(
        string collection)
    {
        if (_collections.TryGetValue(collection, out Dictionary<string, SemanticDocument>? documents))
        {
            return documents;
        }

        documents = [];
        _collections[collection] = documents;

        return documents;
    }
}
