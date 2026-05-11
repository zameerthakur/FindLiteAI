using FindLiteAI.Core.Models;

namespace FindLiteAI.Core.Abstractions;

/// <summary>
/// Defines an abstraction for semantic document storage.
/// </summary>
public interface ISemanticStore
{
    /// <summary>
    /// Adds or updates a document and its embedding vector.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="document">
    /// The searchable document.
    /// </param>
    /// <param name="embedding">
    /// The semantic embedding vector.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpsertAsync(
        string collection,
        SemanticDocument document,
        IReadOnlyList<float> embedding,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates multiple documents and embedding vectors.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="documents">
    /// The searchable documents.
    /// </param>
    /// <param name="embeddings">
    /// The semantic embedding vectors.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task UpsertRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        IReadOnlyCollection<IReadOnlyList<float>> embeddings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all indexed documents and embeddings within a collection.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A collection of indexed documents and embeddings.
    /// </returns>
    Task<IReadOnlyList<(SemanticDocument Document, IReadOnlyList<float> Embedding)>> GetAllAsync(
        string collection,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a document and embedding vector by identifier.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="documentId">
    /// The document identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching document and embedding vector if found; otherwise null.
    /// </returns>
    Task<(SemanticDocument Document, IReadOnlyList<float> Embedding)?> GetByIdAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document from a collection.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="documentId">
    /// The document identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task DeleteAsync(
        string collection,
        string documentId,
        CancellationToken cancellationToken = default);
}
