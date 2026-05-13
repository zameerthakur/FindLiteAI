using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains tests for model installation behavior.
/// </summary>
public sealed class ModelInstallServiceTests
{
    /// <summary>
    /// Verifies that installation returns an existing model directory when it already contains MODEL_INFO.json.
    /// </summary>
    [Fact]
    public async Task InstallAsync_WhenModelAlreadyExists_ShouldReturnExistingModelDirectory()
    {
        string cacheDirectory =
            Directory.CreateTempSubdirectory("findliteai-cache-").FullName;

        string modelDirectory =
            Path.Combine(
                cacheDirectory,
                "all-MiniLM-L6-v2");

        Directory.CreateDirectory(modelDirectory);

        File.WriteAllText(
            Path.Combine(modelDirectory, "MODEL_INFO.json"),
            "{}");

        string installedDirectory =
            await ModelInstallService.InstallAsync(
                FindLiteAIModels.MiniLm,
                cacheDirectory,
                overwrite: false);

        installedDirectory.Should().Be(modelDirectory);
    }

    /// <summary>
    /// Verifies that a missing cache directory value returns a clear exception.
    /// </summary>
    [Fact]
    public async Task InstallAsync_WhenCacheDirectoryIsMissing_ShouldThrowSearchException()
    {
        Func<Task> action = async () =>
            await ModelInstallService.InstallAsync(
                FindLiteAIModels.MiniLm,
                "",
                overwrite: false);

        await action
            .Should()
            .ThrowAsync<SearchException>()
            .WithMessage("*Model cache directory must be configured*");
    }
}
