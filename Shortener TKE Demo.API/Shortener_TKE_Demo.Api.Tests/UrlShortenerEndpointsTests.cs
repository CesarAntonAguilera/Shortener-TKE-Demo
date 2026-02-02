using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Shortener_TKE_Demo.Api.Tests
{
    public sealed class UrlShortenerEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UrlShortenerEndpointsTests(CustomWebApplicationFactory factory)
        {
            // To test 302, we deactivate redirect of HttpClient
            _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Post_Shorten_Returns_200_And_ShortUrl()
        {
            // Arrange
            var request = new { longUrl = "https://example.com/some/very/long/url" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ShortenResponse>();
            body.Should().NotBeNull();
            body!.LongUrl.Should().Be(request.longUrl);
            body.ShortCode.Should().NotBeNullOrWhiteSpace();
            body.ShortUrl.Should().NotBeNullOrWhiteSpace();
            body.ShortUrl.Should().Contain(body.ShortCode);
        }

        [Fact]
        public async Task Post_Shorten_SameUrl_Returns_SameCode_Idempotent()
        {
            // Arrange
            var request = new { longUrl = "https://example.com/idempotent" };

            // Act
            var r1 = await _client.PostAsJsonAsync("/api/shorten", request);
            var b1 = await r1.Content.ReadFromJsonAsync<ShortenResponse>();

            var r2 = await _client.PostAsJsonAsync("/api/shorten", request);
            var b2 = await r2.Content.ReadFromJsonAsync<ShortenResponse>();

            // Assert
            r1.StatusCode.Should().Be(HttpStatusCode.OK);
            r2.StatusCode.Should().Be(HttpStatusCode.OK);

            b1!.ShortCode.Should().Be(b2!.ShortCode);
            b1.ShortUrl.Should().Be(b2.ShortUrl);
        }

        [Fact]
        public async Task Get_ShortCode_Returns_302_With_Location_Header()
        {
            // Arrange
            var longUrl = "https://example.com/redirect-me";
            var createResponse = await _client.PostAsJsonAsync("/api/shorten", new { longUrl });
            var created = await createResponse.Content.ReadFromJsonAsync<ShortenResponse>();
            created.Should().NotBeNull();

            // Act
            var redirectResponse = await _client.GetAsync("/" + created!.ShortCode);

            // Assert
            redirectResponse.StatusCode.Should().Be(HttpStatusCode.Found); // 302
            redirectResponse.Headers.Location.Should().NotBeNull();
            redirectResponse.Headers.Location!.ToString().Should().Be(longUrl);
        }

        [Fact]
        public async Task Post_Shorten_InvalidUrl_Returns_400_ProblemDetails()
        {
            // Arrange
            var request = new { longUrl = "not-a-valid-url" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/shorten", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();
            problem.Should().NotBeNull();
            problem!.Title.Should().NotBeNullOrWhiteSpace();
            problem.Detail.Should().NotBeNullOrWhiteSpace();
        }

        // Local models for deserialize test response
        private sealed class ShortenResponse
        {
            public string LongUrl { get; set; } = default!;
            public string ShortCode { get; set; } = default!;
            public string ShortUrl { get; set; } = default!;
        }

        private sealed class ProblemDetailsResponse
        {
            public string? Title { get; set; }
            public int? Status { get; set; }
            public string? Detail { get; set; }
        }
    }
}
