using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Models;
using FindLiteAI.Storage.LiteDb.Internal;
using LiteDB;

namespace FindLiteAI.Storage.LiteDb;

/// <summary>
/// Provides LiteDB-based semantic document storage.
/// </summary>
public sealed class LiteDbSemanticStore : ISemanticStore, IDisposable
{
    private readonly LiteDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteDbSemanticStore"/> class.
    /// </summary>
    /// <param name="options">
    /// The LiteDB storage configuration.
    /// </param>
    public LiteDbSemanticStore(
        LiteDbOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _database = new LiteDatabase(options.DatabasePath);
    }

    /// <inheritdoc />
    public Task UpsertAsync(
        string collection,
        SemanticDocument document,
        IReadOnlyList<float> embedding,
        CancellationToken cancellationToken = default)
    {
        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            GetCollection(collection);

        LiteDbDocumentEntity entity =
            LiteDbDocumentEntity.Create(
                document,
                embedding);

        liteCollection.Upsert(entity);

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

        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            GetCollection(collection);

        List<LiteDbDocumentEntity> entities =
            documents
                .Zip(embeddings)
                .Select(item =>
                    LiteDbDocumentEntity.Create(
                        item.First,
                        item.Second))
                .ToList();

        liteCollection.Upsert(entities);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)>> GetAllAsync(
        string collection,
        CancellationToken cancellationToken = default)
    {
        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            GetCollection(collection);

        IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)> result =
            liteCollection
                .FindAll()
                .Select(entity =>
                    (
                        entity.ToDocument(),
                        (IReadOnlyList<float>)entity.Embedding
                    ))
                .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<(SemanticDocument Document, IReadOnlyList<float> Embedding)?> GetByIdAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            GetCollection(collection);

        LiteDbDocumentEntity? entity =
            liteCollection.FindById(documentId);

        if (entity is null)
        {
            return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(null);
        }

        return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(
            (
                entity.ToDocument(),
                entity.Embedding
            ));
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            GetCollection(collection);

        liteCollection.Delete(documentId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Releases LiteDB resources.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();
    }

    private ILiteCollection<LiteDbDocumentEntity> GetCollection(
        string collection)
    {
        ILiteCollection<LiteDbDocumentEntity> liteCollection =
            _database.GetCollection<LiteDbDocumentEntity>(
                collection);

        liteCollection.EnsureIndex(
            entity => entity.Id,
            unique: true);

        return liteCollection;
    }
}
