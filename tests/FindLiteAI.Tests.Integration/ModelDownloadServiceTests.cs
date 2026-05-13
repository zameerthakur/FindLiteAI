using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains tests for model download validation behavior.
/// </summary>
public sealed class ModelDownloadServiceTests
{
    /// <summary>
    /// Verifies that a missing package source URL returns a clear exception.
    /// </summary>
    [Fact]
    public async Task DownloadAsync_WhenPackageSourceIsMissing_ShouldThrowSearchException()
    {
        FindLiteAIModelDefinition model = new()
        {
            Id = "test-model",
            DisplayName = "Test Model",
            Profile = "Fast",
            Dimensions = 384,
            MinimumRamGb = 4,
            RecommendedRamGb = 8,
            Source = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2",
            PackageSource = "",
            License = "Apache-2.0"
        };

        Func<Task> action = async () =>
            await ModelDownloadService.DownloadAsync(
                model,
                Path.Combine(
                    Path.GetTempPath(),
                    "test-model.zip"));

        await action
            .Should()
            .ThrowAsync<SearchException>()
            .WithMessage("*Package source URL is not configured*");
    }

    /// <summary>
    /// Verifies that a missing target ZIP path returns a clear exception.
    /// </summary>
    [Fact]
    public async Task DownloadAsync_WhenTargetZipPathIsMissing_ShouldThrowSearchException()
    {
        Func<Task> action = async () =>
            await ModelDownloadService.DownloadAsync(
                FindLiteAIModels.MiniLm,
                "");

        await action
            .Should()
            .ThrowAsync<SearchException>()
            .WithMessage("*Target ZIP file path must be configured*");
    }

    /// <summary>
    /// Verifies that an invalid download URL returns a clear exception.
    /// </summary>
    [Fact]
    public async Task DownloadAsync_WhenUrlDoesNotExist_ShouldThrowSearchException()
    {
        FindLiteAIModelDefinition model = new()
        {
            Id = "missing-model",
            DisplayName = "Missing Model",
            Profile = "Fast",
            Dimensions = 384,
            MinimumRamGb = 4,
            RecommendedRamGb = 8,
            Source = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2",
            PackageSource = "https://github.com/zameerthakur/FindLiteAI/releases/download/v1/does-not-exist.zip",
            License = "Apache-2.0"
        };

        string targetZipPath =
            Path.Combine(
                Path.GetTempPath(),
                $"findliteai-missing-{Guid.NewGuid():N}.zip");

        Func<Task> action = async () =>
            await ModelDownloadService.DownloadAsync(
                model,
                targetZipPath);

        await action
            .Should()
            .ThrowAsync<SearchException>()
            .WithMessage("*Model package download failed*");
    }
}
