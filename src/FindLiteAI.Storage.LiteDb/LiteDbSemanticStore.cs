using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Core.Models;
using FindLiteAI.Storage.LiteDb.Internal;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace FindLiteAI.Storage.LiteDb;

/// <summary>
/// Provides LiteDB-based semantic document storage.
/// </summary>
public sealed class LiteDbSemanticStore : ISemanticStore, IDisposable
{
    private readonly LiteDatabase _database;
    private readonly ILogger<LiteDbSemanticStore> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteDbSemanticStore"/> class.
    /// </summary>
    /// <param name="options">
    /// The LiteDB storage configuration.
    /// </param>
    /// <param name="logger">
    /// The logger instance.
    /// </param>
    public LiteDbSemanticStore(
        LiteDbOptions options,
        ILogger<LiteDbSemanticStore> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger;

        try
        {
            _logger.LogInformation(
                "Opening LiteDB semantic store at '{DatabasePath}'.",
                options.DatabasePath);

            _database = new LiteDatabase(options.DatabasePath);

            _logger.LogInformation(
                "LiteDB semantic store opened successfully.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to open LiteDB semantic store at '{DatabasePath}'.",
                options.DatabasePath);

            throw new SearchException(
                $"Failed to open LiteDB semantic store at '{options.DatabasePath}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public Task UpsertAsync(
        string collection,
        SemanticDocument document,
        IReadOnlyList<float> embedding,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ILiteCollection<LiteDbDocumentEntity> liteCollection =
                GetCollection(collection);

            LiteDbDocumentEntity entity =
                LiteDbDocumentEntity.Create(
                    document,
                    embedding);

            liteCollection.Upsert(entity);

            _logger.LogDebug(
                "Upserted document '{DocumentId}' into LiteDB collection '{Collection}'.",
                document.Id,
                collection);

            return Task.CompletedTask;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to upsert document '{DocumentId}' into LiteDB collection '{Collection}'.",
                document.Id,
                collection);

            throw new SearchException(
                $"Failed to upsert document '{document.Id}' into collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public Task UpsertRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        IReadOnlyCollection<IReadOnlyList<float>> embeddings,
        CancellationToken cancellationToken = default)
    {
        try
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

            _logger.LogDebug(
                "Upserted {DocumentCount} documents into LiteDB collection '{Collection}'.",
                entities.Count,
                collection);

            return Task.CompletedTask;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to upsert documents into LiteDB collection '{Collection}'.",
                collection);

            throw new SearchException(
                $"Failed to upsert documents into collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)>> GetAllAsync(
        string collection,
        CancellationToken cancellationToken = default)
    {
        try
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

            _logger.LogDebug(
                "Retrieved {DocumentCount} documents from LiteDB collection '{Collection}'.",
                result.Count,
                collection);

            return Task.FromResult(result);
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to retrieve documents from LiteDB collection '{Collection}'.",
                collection);

            throw new SearchException(
                $"Failed to retrieve documents from collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public Task<(SemanticDocument Document, IReadOnlyList<float> Embedding)?> GetByIdAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ILiteCollection<LiteDbDocumentEntity> liteCollection =
                GetCollection(collection);

            LiteDbDocumentEntity? entity =
                liteCollection.FindById(documentId);

            if (entity is null)
            {
                _logger.LogDebug(
                    "Document '{DocumentId}' was not found in LiteDB collection '{Collection}'.",
                    documentId,
                    collection);

                return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(null);
            }

            _logger.LogDebug(
                "Retrieved document '{DocumentId}' from LiteDB collection '{Collection}'.",
                documentId,
                collection);

            return Task.FromResult<(SemanticDocument Document, IReadOnlyList<float> Embedding)?>(
                (
                    entity.ToDocument(),
                    entity.Embedding
                ));
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to retrieve document '{DocumentId}' from LiteDB collection '{Collection}'.",
                documentId,
                collection);

            throw new SearchException(
                $"Failed to retrieve document '{documentId}' from collection '{collection}'.",
                exception);
        }
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ILiteCollection<LiteDbDocumentEntity> liteCollection =
                GetCollection(collection);

            liteCollection.Delete(documentId);

            _logger.LogDebug(
                "Deleted document '{DocumentId}' from LiteDB collection '{Collection}'.",
                documentId,
                collection);

            return Task.CompletedTask;
        }
        catch (Exception exception) when (exception is not SearchException)
        {
            _logger.LogError(
                exception,
                "Failed to delete document '{DocumentId}' from LiteDB collection '{Collection}'.",
                documentId,
                collection);

            throw new SearchException(
                $"Failed to delete document '{documentId}' from collection '{collection}'.",
                exception);
        }
    }

    /// <summary>
    /// Releases LiteDB resources.
    /// </summary>
    public void Dispose()
    {
        _database.Dispose();

        _logger.LogDebug("Disposed LiteDB semantic store.");
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
