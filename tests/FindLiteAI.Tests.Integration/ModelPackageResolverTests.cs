using System.Text.Json;
using FindLiteAI.Core.Exceptions;
using FindLiteAI.Embeddings.Onnx;
using FindLiteAI.Embeddings.Onnx.Internal;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for FindLiteAI model package resolution.
/// </summary>
public sealed class ModelPackageResolverTests
{
    /// <summary>
    /// Verifies that a valid extracted model package resolves ONNX provider options.
    /// </summary>
    [Fact]
    public void Resolve_WhenModelPackageIsValid_ShouldReturnProviderOptions()
    {
        string modelDirectory = CreateModelPackage();

        OnnxEmbeddingProviderOptions options =
            ModelPackageResolver.Resolve(
                modelDirectory,
                maxTokenLength: 128,
                warmupOnLoad: false);

        options.ModelPath.Should().Be(
            Path.Combine(modelDirectory, "model.onnx"));

        options.VocabularyPath.Should().Be(
            Path.Combine(modelDirectory, "vocab.txt"));

        options.MaxTokenLength.Should().Be(128);

        options.WarmupOnLoad.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that missing MODEL_INFO.json returns a clear exception.
    /// </summary>
    [Fact]
    public void Resolve_WhenModelInfoIsMissing_ShouldThrowSearchException()
    {
        string modelDirectory =
            Directory.CreateTempSubdirectory("findliteai-model-").FullName;

        Action action = () =>
            ModelPackageResolver.Resolve(modelDirectory);

        action
            .Should()
            .Throw<SearchException>()
            .WithMessage("*MODEL_INFO.json*");
    }

    /// <summary>
    /// Verifies that missing runtime files return a clear exception.
    /// </summary>
    [Fact]
    public void Resolve_WhenModelFileIsMissing_ShouldThrowSearchException()
    {
        string modelDirectory =
            Directory.CreateTempSubdirectory("findliteai-model-").FullName;

        WriteModelInfo(modelDirectory);

        File.WriteAllText(
            Path.Combine(modelDirectory, "vocab.txt"),
            "test");

        Action action = () =>
            ModelPackageResolver.Resolve(modelDirectory);

        action
            .Should()
            .Throw<SearchException>()
            .WithMessage("*ONNX model file*");
    }

    /// <summary>
    /// Creates a temporary valid model package directory.
    /// </summary>
    /// <returns>
    /// A temporary model package directory.
    /// </returns>
    private static string CreateModelPackage()
    {
        string modelDirectory =
            Directory.CreateTempSubdirectory("findliteai-model-").FullName;

        WriteModelInfo(modelDirectory);

        File.WriteAllBytes(
            Path.Combine(modelDirectory, "model.onnx"),
            [1, 2, 3]);

        File.WriteAllText(
            Path.Combine(modelDirectory, "vocab.txt"),
            "test");

        return modelDirectory;
    }

    /// <summary>
    /// Writes MODEL_INFO.json into a model package directory.
    /// </summary>
    /// <param name="modelDirectory">
    /// The target model directory.
    /// </param>
    private static void WriteModelInfo(
        string modelDirectory)
    {
        ModelPackageInfo info = new()
        {
            Id = "test-model",
            DisplayName = "Test Model",
            Profile = "Fast",
            Dimensions = 384,
            Runtime = "ONNX",
            OnnxPath = "model.onnx",
            TokenizerPath = "vocab.txt",
            Pooling = "mean",
            Source = "https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2",
            PackageSource = "https://github.com/zameerthakur/FindLiteAI-Models/releases/download/v1/all-MiniLM-L6-v2.zip",
            OptimizedFor = "FindLiteAI",
            License = "Apache-2.0"
        };

        string json =
            JsonSerializer.Serialize(
                info,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            Path.Combine(modelDirectory, "MODEL_INFO.json"),
            json);
    }
}
