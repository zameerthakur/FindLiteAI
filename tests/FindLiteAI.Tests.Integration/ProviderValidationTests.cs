using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Storage.LiteDb;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains validation tests for infrastructure providers.
/// </summary>
public sealed class ProviderValidationTests
{
    /// <summary>
    /// Verifies that the ONNX provider throws a clear exception when the model path is missing.
    /// </summary>
    [Fact]
    public void OnnxEmbeddingProvider_WhenModelPathIsMissing_ShouldThrowSearchException()
    {
        Action action = () =>
            new OnnxEmbeddingProvider(
                new OnnxEmbeddingProviderOptions
                {
                    ModelPath = @"D:\missing-model.onnx",
                    VocabularyPath = @"D:\missing-vocab.txt"
                },
                NullLogger<OnnxEmbeddingProvider>.Instance);

        action
            .Should()
            .Throw<SearchException>()
            .WithMessage("*ONNX model file was not found*");
    }

    /// <summary>
    /// Verifies that the LiteDB provider throws a clear exception when the directory does not exist.
    /// </summary>
    [Fact]
    public void LiteDbSemanticStore_WhenDirectoryDoesNotExist_ShouldThrowSearchException()
    {
        Action action = () =>
            new LiteDbSemanticStore(
                new LiteDbOptions
                {
                    DatabasePath = @"D:\folder-that-does-not-exist\findliteai.db"
                },
                NullLogger<LiteDbSemanticStore>.Instance);

        action
            .Should()
            .Throw<SearchException>()
            .WithMessage("*LiteDB database directory does not exist*");
    }
}
