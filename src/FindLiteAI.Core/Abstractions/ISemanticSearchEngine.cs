using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;
using FindLiteAI.Core.Results;

namespace FindLiteAI.Core.Abstractions;

/// <summary>
/// Defines the primary semantic search engine contract.
/// </summary>
public interface ISemanticSearchEngine
{
    /// <summary>
    /// Adds or updates a searchable document within a collection.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="document">
    /// The document to index.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task AddAsync(
        string collection,
        SemanticDocument document,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates multiple searchable documents within a collection.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="documents">
    /// The documents to index.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task AddRangeAsync(
        string collection,
        IReadOnlyCollection<SemanticDocument> documents,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches documents within a collection.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="query">
    /// The search query text.
    /// </param>
    /// <param name="options">
    /// Optional search behavior configuration.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A ranked list of matching search results.
    /// </returns>
    Task<IReadOnlyList<SearchResult>> SearchAsync(
        string collection,
        string query,
        SearchOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds documents similar to an existing indexed document.
    /// </summary>
    /// <param name="collection">
    /// The target collection name.
    /// </param>
    /// <param name="documentId">
    /// The source document identifier.
    /// </param>
    /// <param name="options">
    /// Optional search behavior configuration.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A ranked list of similar documents.
    /// </returns>
    Task<IReadOnlyList<SearchResult>> FindSimilarAsync(
        string collection,
        string documentId,
        SearchOptions? options = null,
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
