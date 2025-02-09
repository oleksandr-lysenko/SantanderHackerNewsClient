using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using HackerNewsClient.Services;

[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly HackerNewsService _hackerNewsService;
    private readonly ILogger<HackerNewsController> _logger;
    private const string BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";

    public HackerNewsController(IMemoryCache cache, HackerNewsService hackerNewsService, ILogger<HackerNewsController> logger)
    {
        _cache = cache;
        _hackerNewsService = hackerNewsService;
        _logger = logger;
    }

    [HttpGet("beststories")]
    public async Task<IActionResult> GetBestStories([FromQuery] int n)
    {
        try
        {
            if (!_cache.TryGetValue(BestStoriesUrl, out int[]? storyIds))
            {
                storyIds = await _hackerNewsService.GetBestStoryIdsAsync();
                if (storyIds == null) return Problem("Failed to retrieve story IDs");

                _cache.Set(BestStoriesUrl, storyIds, TimeSpan.FromMinutes(10));
            }

            var tasks = storyIds.Take(n).Select(async id =>
            {
                try
                {
                    return await _hackerNewsService.GetStoryDetailsAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to fetch story with ID {id}");
                    return null;
                }
            });

            // var stories = (await Task.WhenAll(tasks)).Where(story => story != null).ToList();
            var stories = await Task.WhenAll(tasks);

            var ordered_stories = stories
                    .Where(story => story != null)
                    .OrderByDescending(story => story.Score)
                    .Select(story => new
                    {
                        title = story.Title,
                        uri = story.Url,
                        postedBy = story.By,
                        time = DateTimeOffset.FromUnixTimeSeconds(story.Time).ToString("o"),
                        score = story.Score,
                        commentCount = story.Descendants
                    })
                    .ToList();

            return Ok(ordered_stories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching best stories");
            return StatusCode(500, "Internal server error");
        }
    }
}
