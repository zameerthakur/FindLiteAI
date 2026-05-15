using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for the ONNX embedding provider.
/// </summary>
public sealed class OnnxEmbeddingProviderTests
{
    private static string CreateCacheDirectory()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "FindLiteAI",
            "Tests",
            Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Verifies that the ONNX embedding provider generates a 384-dimensional embedding.
    /// </summary>
    [Fact]
    public async Task GenerateEmbeddingAsync_WhenUsingMiniLmModel_ShouldReturn384Dimensions()
    {
        using OnnxEmbeddingProvider provider =
            await CreateProviderAsync();

        IReadOnlyList<float> embedding =
            await provider.GenerateEmbeddingAsync(
                "SFTP authentication failed for remote user.");

        embedding.Should().HaveCount(384);

        embedding.Should().Contain(value => value != 0);
    }

    /// <summary>
    /// Verifies that different input texts generate valid but different embedding vectors.
    /// </summary>
    [Fact]
    public async Task GenerateEmbeddingAsync_WhenTextsAreDifferent_ShouldReturnDifferentVectors()
    {
        using OnnxEmbeddingProvider provider =
            await CreateProviderAsync();

        IReadOnlyList<float> firstEmbedding =
            await provider.GenerateEmbeddingAsync(
                "SFTP authentication failed.");

        IReadOnlyList<float> secondEmbedding =
            await provider.GenerateEmbeddingAsync(
                "SMTP email relay accepted the message.");

        firstEmbedding.Should().HaveCount(384);

        secondEmbedding.Should().HaveCount(384);

        firstEmbedding.Should().NotEqual(secondEmbedding);
    }

    /// <summary>
    /// Verifies that batch embedding generation returns one vector per input text.
    /// </summary>
    [Fact]
    public async Task GenerateEmbeddingsAsync_WhenMultipleTextsAreProvided_ShouldReturnMatchingEmbeddingCount()
    {
        using OnnxEmbeddingProvider provider =
            await CreateProviderAsync();

        IReadOnlyList<IReadOnlyList<float>> embeddings =
            await provider.GenerateEmbeddingsAsync(
            [
                "SFTP authentication failed.",
                "SQL database timeout occurred.",
                "SMTP email relay accepted the message."
            ]);

        embeddings.Should().HaveCount(3);

        embeddings.Should().OnlyContain(embedding => embedding.Count == 384);
    }

    /// <summary>
    /// Creates an ONNX embedding provider configured from an installed MiniLM model package.
    /// </summary>
    /// <returns>
    /// A configured ONNX embedding provider.
    /// </returns>
    private static async Task<OnnxEmbeddingProvider> CreateProviderAsync()
    {
        string cacheDirectory = CreateCacheDirectory();

        await ModelInstallService.InstallAsync(
            FindLiteAIModels.MiniLm,
            cacheDirectory,
            overwrite: false);

        string modelDirectory =
            Path.Combine(
                cacheDirectory,
                FindLiteAIModels.MiniLm.Id);

        return OnnxEmbeddingProviderFactory.FromModelPackage(
            modelDirectory,
            NullLogger<OnnxEmbeddingProvider>.Instance);
    }
}
