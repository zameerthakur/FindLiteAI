using System.Net;
using System.Net.Http.Json;
using FindLiteAI.AspNetCore;
using FindLiteAI.AspNetCore.Models;
using FindLiteAI.Core.Abstractions;
using FindLiteAI.Tests.AspNetCore.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FindLiteAI.Tests.AspNetCore;

/// <summary>
/// Contains tests for FindLiteAI ASP.NET Core endpoints.
/// </summary>
public sealed class FindLiteAIEndpointTests
{
    /// <summary>
    /// Verifies that a document can be added and searched through the ASP.NET Core endpoints.
    /// </summary>
    [Fact]
    public async Task MapFindLiteAI_WhenDocumentIsAdded_ShouldReturnSearchResults()
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder();

        builder.WebHost.UseUrls("http://127.0.0.1:0");

        builder.Services.AddSingleton<ISemanticSearchEngine>(
            new FakeSemanticSearchEngine());

        WebApplication app =
            builder.Build();

        app.MapFindLiteAI();

        await app.StartAsync();

        try
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri(app.Urls.Single())
            };

            HttpResponseMessage addResponse =
                await client.PostAsJsonAsync(
                    "/api/findliteai/collections/logs/documents",
                    new AddDocumentRequest
                    {
                        Id = "log-1",
                        Text = "SFTP authentication failed."
                    });

            addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            HttpResponseMessage searchResponse =
                await client.PostAsJsonAsync(
                    "/api/findliteai/collections/logs/search",
                    new SearchRequest
                    {
                        Query = "login issue",
                        MinimumScore = 0.1,
                        MaxResults = 10
                    });

            searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            IReadOnlyList<SearchResponse>? results =
                await searchResponse.Content
                    .ReadFromJsonAsync<IReadOnlyList<SearchResponse>>();

            results.Should().NotBeNull();

            results!.Should().HaveCount(1);

            results[0].Id.Should().Be("log-1");
        }
        finally
        {
            await app.StopAsync();
            await app.DisposeAsync();
        }
    }
}
