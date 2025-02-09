using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsClient.Services
{
    public class HackerNewsService
    {
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";
        private const int CacheTTL = 10;

        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HackerNewsService> _logger;

        public HackerNewsService(IMemoryCache cache, IHttpClientFactory clientFactory, ILogger<HackerNewsService> logger)
        {
            _cache = cache;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<int[]> GetBestStoryIdsAsync()
        {
            var url = $"{BaseUrl}/beststories.json";

            if (_cache.TryGetValue(url, out int[] cached_storyIds))
            {
                _logger.LogDebug("Returning best story IDs from cache.");
                return cached_storyIds;
            }
            _logger.LogDebug("Fetching best story IDs from the server.");

            var client = _clientFactory.CreateClient();
            var bestStoriesResponse = await client.GetStringAsync(url);

            if (string.IsNullOrEmpty(bestStoriesResponse))
            {
                var errMsg = "The response from the server is empty.";
                _logger.LogError(errMsg);
                throw new Exception(errMsg);
            }

            try
            {
                var storyIds = JsonSerializer.Deserialize<int[]>(bestStoriesResponse);
                if (storyIds == null)
                    throw new JsonException("Deserialization failed, story is null.");

                _cache.Set(url, storyIds, TimeSpan.FromMinutes(CacheTTL));
                return storyIds;
            }
            catch (JsonException exc)
            {
                _logger.LogError($"Failed to deserialize the response as int[]. {exc.Message}");
                throw;
            }
        }

        public async Task<HackerNewsStory> GetStoryDetailsAsync(int storyId)
        {
            var url = $"{BaseUrl}/item/{storyId}.json";

            if (_cache.TryGetValue(url, out HackerNewsStory cached_story))
            {
                _logger.LogDebug($"Returning story with ID {storyId} from cache.");
                return cached_story;
            }
            _logger.LogDebug($"Fetching story with ID {storyId} from the server.");

            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync(url);

            try
            {
                var story = JsonSerializer.Deserialize<HackerNewsStory>(response);
                if (story == null)
                    throw new JsonException("Deserialization failed, story is null.");

                _cache.Set(url, story, TimeSpan.FromMinutes(CacheTTL));
                return story;
            }
            catch (JsonException exc)
            {
                _logger.LogError($"Failed to deserialize the response as HackerNewsStory. {exc.Message}");
                throw;
            }
        }
    }
}