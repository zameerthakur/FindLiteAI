namespace FindLiteAI.Core.Abstractions;

/// <summary>
/// Defines an abstraction for generating semantic embeddings from text.
/// </summary>
public interface IEmbeddingProvider
{
    /// <summary>
    /// Generates a semantic embedding vector for the provided text.
    /// </summary>
    /// <param name="text">
    /// The source text.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A semantic embedding vector.
    /// </returns>
    Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates semantic embedding vectors for multiple texts.
    /// </summary>
    /// <param name="texts">
    /// The source texts.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A collection of semantic embedding vectors.
    /// </returns>
    Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IReadOnlyCollection<string> texts,
        CancellationToken cancellationToken = default);
}
