using ConferenceDTO;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
namespace FrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    protected readonly IApiClient _apiClient;
    public IndexModel(IApiClient apiClient, ILogger<IndexModel> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public IEnumerable<IGrouping<DateTimeOffset?, SessionResponse>> Sessions { get; set; }

    public IEnumerable<(int Offset, DayOfWeek? DayofWeek)> DayOffsets { get; set; }

    public int CurrentDayOffset { get; set; }
    public async Task OnGet(int day = 0)
    {
        CurrentDayOffset = day;

        var sessions = await _apiClient.GetSessionsAsync();

        var startDate = sessions.Min(s => s.StartTime?.Date);

        var offset = 0;
        DayOffsets = sessions.Select(s => s.StartTime?.Date)
                             .Distinct()
                             .OrderBy(d => d)
                             .Select(day => (offset++, day?.DayOfWeek));

        var filterDate = startDate?.AddDays(day);

        Sessions = sessions.Where(s => s.StartTime?.Date == filterDate)
                           .OrderBy(s => s.TrackId)
                           .GroupBy(s => s.StartTime)
                           .OrderBy(g => g.Key);
    }
}
