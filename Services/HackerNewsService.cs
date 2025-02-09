using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HackerNewsClient.Services
{
    public class HackerNewsService
    {
        private readonly IHttpClientFactory _clientFactory;

        public HackerNewsService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<int[]> GetBestStoryIdsAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/beststories.json");
            return JsonSerializer.Deserialize<int[]>(response);
        }

        public async Task<HackerNewsStory> GetStoryDetailsAsync(int storyId)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json");
            return JsonSerializer.Deserialize<HackerNewsStory>(response);
        }
    }
}