using FindLiteAI.Core.Abstractions;
using FindLiteAI.Core.Options;

namespace FindLiteAI;

/// <summary>
/// Provides simplified factory methods for creating FindLiteAI engines.
/// </summary>
public static class FindLiteAIFactory
{
    /// <summary>
    /// Creates and initializes a semantic search engine instance.
    /// </summary>
    /// <param name="options">
    /// Optional FindLiteAI engine configuration.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// An initialized semantic search engine instance.
    /// </returns>
    public static Task<ISemanticSearchEngine> CreateAsync(
        FindLiteAIOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(
            "Provider initialization has not been implemented yet.");
    }
}
