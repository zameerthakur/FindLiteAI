using FindLiteAI.Core.Models;
using FindLiteAI.Core.Options;

namespace FindLiteAI.Internal.Validation;

/// <summary>
/// Provides centralized validation for FindLiteAI engine operations.
/// </summary>
internal static class SearchEngineValidator
{
    /// <summary>
    /// Validates a collection name.
    /// </summary>
    /// <param name="collection">
    /// The collection name to validate.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the collection name is invalid.
    /// </exception>
    public static void ValidateCollection(string collection)
    {
        if (string.IsNullOrWhiteSpace(collection))
        {
            throw new ArgumentException(
                "Collection name cannot be null or empty.",
                nameof(collection));
        }
    }

    /// <summary>
    /// Validates a search query.
    /// </summary>
    /// <param name="query">
    /// The query text to validate.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the query is invalid.
    /// </exception>
    public static void ValidateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Search query cannot be null or empty.",
                nameof(query));
        }
    }

    /// <summary>
    /// Validates a semantic document.
    /// </summary>
    /// <param name="document">
    /// The document to validate.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the document is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the document contains invalid data.
    /// </exception>
    public static void ValidateDocument(SemanticDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (string.IsNullOrWhiteSpace(document.Id))
        {
            throw new ArgumentException(
                "Document identifier cannot be null or empty.",
                nameof(document));
        }

        if (string.IsNullOrWhiteSpace(document.Text))
        {
            throw new ArgumentException(
                "Document text cannot be null or empty.",
                nameof(document));
        }
    }

    /// <summary>
    /// Validates search options.
    /// </summary>
    /// <param name="options">
    /// The search options to validate.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the options are null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when option values are invalid.
    /// </exception>
    public static void ValidateSearchOptions(SearchOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.MaxResults <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options.MaxResults),
                "MaxResults must be greater than zero.");
        }

        if (options.MinimumScore < 0 ||
            options.MinimumScore > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options.MinimumScore),
                "MinimumScore must be between 0 and 1.");
        }
    }
}
