using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace HackerNewsClient.Services.Tests
{
    public class HackerNewsServiceTest
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<IHttpClientFactory> _mockClientFactory;
        private readonly Mock<ILogger<HackerNewsService>> _mockLogger;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTest()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<HackerNewsService>>();
            _service = new HackerNewsService(_mockCache.Object, _mockClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetBestStoryIdsAsync_ReturnsFromCache()
        {
            var cachedStoryIds = new int[] { 1, 2, 3 };
            object cacheValue = cachedStoryIds;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.GetBestStoryIdsAsync();

            Assert.Equal(cachedStoryIds, result);
        }

        [Fact]
        public async Task GetBestStoryIdsAsync_FetchesFromServerAndCaches()
        {
            var storyIds = new int[] { 1, 2, 3 };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(storyIds))
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var cacheEntry = new Mock<ICacheEntry>();
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            var result = await _service.GetBestStoryIdsAsync();
            // var result = new int[] {3, 9};

            Assert.Equal(storyIds, result);
            cacheEntry.VerifySet(x => x.Value = storyIds, Times.Once);
        }

        [Fact]
        public async Task GetBestStoryIdsAsync_ThrowsExceptionOnEmptyResponse()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(string.Empty)
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            await Assert.ThrowsAsync<Exception>(() => _service.GetBestStoryIdsAsync());
        }

        [Fact]
        public async Task GetBestStoryIdsAsync_ThrowsExceptionOnDeserializationFailure()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            await Assert.ThrowsAsync<JsonException>(() => _service.GetBestStoryIdsAsync());
        }

        [Fact]
        public async Task GetStoryDetailsAsync_ReturnsFromCache()
        {
            var cachedStory = new HackerNewsStory { Id = 1, Title = "Test Story" };
            object cacheValue = cachedStory;
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue)).Returns(true);

            var result = await _service.GetStoryDetailsAsync(1);

            Assert.Equal(cachedStory, result);
        }

        [Fact]
        public async Task GetStoryDetailsAsync_FetchesFromServerAndCaches()
        {
            var story = new HackerNewsStory { Id = 1, Title = "Test Story" };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(story))
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var cacheEntry = new Mock<ICacheEntry>();
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            var result = await _service.GetStoryDetailsAsync(1);

            Assert.Equal(story, result);
            cacheEntry.VerifySet(x => x.Value = story, Times.Once);
        }

        [Fact]
        public async Task GetStoryDetailsAsync_ThrowsExceptionOnDeserializationFailure()
        {
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json")
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            await Assert.ThrowsAsync<JsonException>(() => _service.GetStoryDetailsAsync(1));
        }
    }
}