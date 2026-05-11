using FindLiteAI.Core.Abstractions;

namespace FindLiteAI.Tests.Core.Fakes;

/// <summary>
/// Provides deterministic fake embeddings for unit tests.
/// </summary>
internal sealed class FakeEmbeddingProvider : IEmbeddingProvider
{
    /// <inheritdoc />
    public Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<float> embedding = CreateEmbedding(text);

        return Task.FromResult(embedding);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<IReadOnlyList<float>>> GenerateEmbeddingsAsync(
        IReadOnlyCollection<string> texts,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<IReadOnlyList<float>> embeddings =
            texts
                .Select(CreateEmbedding)
                .ToList();

        return Task.FromResult(embeddings);
    }

    private static IReadOnlyList<float> CreateEmbedding(string text)
    {
        string normalizedText = text.ToLowerInvariant();

        float loginScore = ContainsAny(
            normalizedText,
            "login",
            "authentication",
            "credentials",
            "password",
            "sftp",
            "ssh")
                ? 1f
                : 0f;

        float databaseScore = ContainsAny(
            normalizedText,
            "database",
            "sql",
            "query",
            "timeout",
            "connection")
                ? 1f
                : 0f;

        float emailScore = ContainsAny(
            normalizedText,
            "email",
            "smtp",
            "mail",
            "relay",
            "notification")
                ? 1f
                : 0f;

        return new[]
        {
            loginScore,
            databaseScore,
            emailScore
        };
    }

    private static bool ContainsAny(
        string text,
        params string[] terms)
    {
        return terms.Any(term =>
            text.Contains(
                term,
                StringComparison.OrdinalIgnoreCase));
    }
}
