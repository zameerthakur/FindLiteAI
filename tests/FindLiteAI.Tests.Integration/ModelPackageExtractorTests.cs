using System.IO.Compression;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for model package ZIP extraction.
/// </summary>
public sealed class ModelPackageExtractorTests
{
    /// <summary>
    /// Verifies that a valid ZIP package can be extracted.
    /// </summary>
    [Fact]
    public void Extract_WhenZipIsValid_ShouldExtractFiles()
    {
        string sourceDirectory =
            Directory.CreateTempSubdirectory("findliteai-zip-source-").FullName;

        string zipPath =
            Path.Combine(
                Path.GetTempPath(),
                $"findliteai-model-{Guid.NewGuid():N}.zip");

        string targetDirectory =
            Path.Combine(
                Path.GetTempPath(),
                $"findliteai-extract-{Guid.NewGuid():N}");

        File.WriteAllText(
            Path.Combine(sourceDirectory, "MODEL_INFO.json"),
            "{}");

        File.WriteAllText(
            Path.Combine(sourceDirectory, "vocab.txt"),
            "test");

        ZipFile.CreateFromDirectory(
            sourceDirectory,
            zipPath);

        string extractedDirectory =
            ModelPackageExtractor.Extract(
                zipPath,
                targetDirectory);

        extractedDirectory.Should().Be(targetDirectory);

        File.Exists(
                Path.Combine(targetDirectory, "MODEL_INFO.json"))
            .Should()
            .BeTrue();

        File.Exists(
                Path.Combine(targetDirectory, "vocab.txt"))
            .Should()
            .BeTrue();
    }

    /// <summary>
    /// Verifies that a missing ZIP file returns a clear exception.
    /// </summary>
    [Fact]
    public void Extract_WhenZipIsMissing_ShouldThrowSearchException()
    {
        Action action = () =>
            ModelPackageExtractor.Extract(
                @"D:\missing-model.zip",
                Path.Combine(
                    Path.GetTempPath(),
                    $"findliteai-extract-{Guid.NewGuid():N}"));

        action
            .Should()
            .Throw<SearchException>()
            .WithMessage("*Model ZIP file was not found*");
    }

    /// <summary>
    /// Verifies that an existing target directory is reused when overwrite is false.
    /// </summary>
    [Fact]
    public void Extract_WhenTargetExistsAndOverwriteIsFalse_ShouldReturnExistingDirectory()
    {
        string targetDirectory =
            Directory.CreateTempSubdirectory("findliteai-existing-").FullName;

        string sourceDirectory =
            Directory.CreateTempSubdirectory("findliteai-zip-source-").FullName;

        string zipPath =
            Path.Combine(
                Path.GetTempPath(),
                $"findliteai-model-{Guid.NewGuid():N}.zip");

        File.WriteAllText(
            Path.Combine(sourceDirectory, "MODEL_INFO.json"),
            "{}");

        ZipFile.CreateFromDirectory(
            sourceDirectory,
            zipPath);

        string extractedDirectory =
            ModelPackageExtractor.Extract(
                zipPath,
                targetDirectory,
                overwrite: false);

        extractedDirectory.Should().Be(targetDirectory);
    }
}
