namespace Attendances.Application.Fetching.Interfaces;

public interface IGlobalFetchService
{
    Task FetchExternalAsync(CancellationToken stoppingToken);
}