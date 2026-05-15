using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;

namespace FindLiteAI.Tests.Core.Fakes;

/// <summary>
/// Provides an in-memory semantic store for unit tests.
/// </summary>
internal sealed class InMemorySemanticStore : ISemanticStore
{
    private readonly Dictionary<string, Dictionary<string, StoredItem>> _collections = [];

    /// <inheritdoc />
    public Task UpsertAsync(
        string collection,
        SemanticDocument document,
        IReadOnlyList<float> embedding,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, StoredItem> documents = GetOrCreateCollection(collection);

        documents[document.Id!] = new StoredItem(
            document,
            embedding);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpsertRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        IReadOnlyCollection<IReadOnlyList<float>> embeddings,
        CancellationToken cancellationToken = default)
    {
        if (documents.Count != embeddings.Count)
        {
            throw new ArgumentException(
                "Document count and embedding count must match.",
                nameof(embeddings));
        }

        Dictionary<string, StoredItem> targetCollection =
            GetOrCreateCollection(collection);

        foreach ((SemanticDocument document, IReadOnlyList<float> embedding) in
                 documents.Zip(embeddings))
        {
            targetCollection[document.Id] = new StoredItem(
                document,
                embedding);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)>> GetAllAsync(
        string collection,
        CancellationToken cancellationToken = default)
    {
        if (!_collections.TryGetValue(collection, out Dictionary<string, StoredItem>? documents))
        {
            return Task.FromResult<IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)>>([]);
        }

        IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> result =
            documents
                .Values
                .Select(item => (item.Document, item.Embedding))
                .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<(SemanticDocument Document, IReadOnlyList<float> Embedding)?> GetByIdAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (!_collections.TryGetValue(collection, out Dictionary<string, StoredItem>? documents))
        {
            return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(null);
        }

        if (!documents.TryGetValue(documentId, out StoredItem? item))
        {
            return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(null);
        }

        return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(
            (item.Document, item.Embedding));
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        if (_collections.TryGetValue(collection, out Dictionary<string, StoredItem>? documents))
        {
            documents.Remove(documentId);
        }

        return Task.CompletedTask;
    }

    private Dictionary<string, StoredItem> GetOrCreateCollection(string collection)
    {
        if (_collections.TryGetValue(collection, out Dictionary<string, StoredItem>? documents))
        {
            return documents;
        }

        documents = [];
        _collections[collection] = documents;

        return documents;
    }

    private sealed record StoredItem(
        SemanticDocument Document,
        IReadOnlyList<float> Embedding);
}
