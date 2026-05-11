using FindLiteAI.Core.Abstractions;
using FindLiteAI.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace FindLiteAI.Tests.Integration;

/// <summary>
/// Contains integration tests for FindLiteAI dependency injection registration.
/// </summary>
public sealed class FindLiteAIDependencyInjectionTests
{
    private const string ModelPath =
        @"D:\AIModels\FindLiteAI\all-MiniLM-L6-v2\model.onnx";

    /// <summary>
    /// Verifies that FindLiteAI services can be registered and resolved through dependency injection.
    /// </summary>
    [Fact]
    public void AddFindLiteAI_WhenConfigured_ShouldResolveSemanticSearchEngine()
    {
        string databasePath = Path.Combine(
            Path.GetTempPath(),
            $"findliteai-di-{Guid.NewGuid():N}.db");

        ServiceCollection services = new();

        services.AddFindLiteAI(options =>
        {
            options.DatabasePath = databasePath;
            options.ModelPath = ModelPath;
        });

        using ServiceProvider provider = services.BuildServiceProvider();

        ISemanticSearchEngine engine =
            provider.GetRequiredService<ISemanticSearchEngine>();

        engine.Should().NotBeNull();
    }
}
