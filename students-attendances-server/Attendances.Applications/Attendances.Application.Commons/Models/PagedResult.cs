namespace Attendances.Application.Commons.Models;

public class PagedResult<TItem>
{
    public required IReadOnlyList<TItem> Items { get; set; }
    public required long TotalCount { get; set; }
}