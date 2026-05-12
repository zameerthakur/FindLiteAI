using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains tests for FindLiteAI model cache path helpers.
/// </summary>
public sealed class ModelCachePathsTests
{
    /// <summary>
    /// Verifies that the user cache directory contains the FindLiteAI models folder.
    /// </summary>
    [Fact]
    public void GetUserCacheDirectory_WhenCalled_ShouldReturnFindLiteAIModelsPath()
    {
        string cacheDirectory =
            ModelCachePaths.GetUserCacheDirectory();

        cacheDirectory.Should().Contain("FindLiteAI");

        cacheDirectory.Should().Contain("Models");
    }

    /// <summary>
    /// Verifies that a model-specific cache directory includes the model identifier.
    /// </summary>
    [Fact]
    public void GetUserModelDirectory_WhenModelIdIsProvided_ShouldReturnModelSpecificPath()
    {
        string modelDirectory =
            ModelCachePaths.GetUserModelDirectory("minilm");

        modelDirectory.Should().Contain("FindLiteAI");

        modelDirectory.Should().Contain("Models");

        modelDirectory.Should().Contain("minilm");
    }

    /// <summary>
    /// Verifies that an empty model identifier throws an exception.
    /// </summary>
    [Fact]
    public void GetUserModelDirectory_WhenModelIdIsEmpty_ShouldThrowArgumentException()
    {
        Action action = () =>
            ModelCachePaths.GetUserModelDirectory("");

        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("*Model identifier cannot be null or empty*");
    }
}
