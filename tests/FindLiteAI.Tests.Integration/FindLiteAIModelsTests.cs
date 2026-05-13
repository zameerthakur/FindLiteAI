using FindLiteAI.Embeddings.Onnx;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains tests for the built-in FindLiteAI model registry.
/// </summary>
public sealed class FindLiteAIModelsTests
{
    /// <summary>
    /// Verifies that all built-in model definitions are available.
    /// </summary>
    [Fact]
    public void GetAll_WhenCalled_ShouldReturnAllBuiltInModels()
    {
        IReadOnlyList<FindLiteAIModelDefinition> models =
            FindLiteAIModels.GetAll();

        models.Should().HaveCount(3);

        models.Should().Contain(model =>
            model.Id == "all-MiniLM-L6-v2");

        models.Should().Contain(model =>
            model.Id == "all-mpnet-base-v2");

        models.Should().Contain(model =>
            model.Id == "arctic-embed-xs");
    }

    /// <summary>
    /// Verifies that all package source URLs are configured.
    /// </summary>
    [Fact]
    public void GetAll_WhenCalled_ShouldContainPackageSourceUrls()
    {
        IReadOnlyList<FindLiteAIModelDefinition> models =
            FindLiteAIModels.GetAll();

        models.Should().OnlyContain(model =>
            !string.IsNullOrWhiteSpace(model.PackageSource));

        models.Should().OnlyContain(model =>
            model.PackageSource.StartsWith(
                "https://github.com/",
                StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Verifies that all built-in models contain valid RAM guidance.
    /// </summary>
    [Fact]
    public void GetAll_WhenCalled_ShouldContainValidRamGuidance()
    {
        IReadOnlyList<FindLiteAIModelDefinition> models =
            FindLiteAIModels.GetAll();

        models.Should().OnlyContain(model =>
            model.MinimumRamGb > 0);

        models.Should().OnlyContain(model =>
            model.RecommendedRamGb >= model.MinimumRamGb);
    }

    /// <summary>
    /// Verifies that all built-in models contain valid dimension metadata.
    /// </summary>
    [Fact]
    public void GetAll_WhenCalled_ShouldContainValidDimensions()
    {
        IReadOnlyList<FindLiteAIModelDefinition> models =
            FindLiteAIModels.GetAll();

        models.Should().OnlyContain(model =>
            model.Dimensions > 0);
    }
}
