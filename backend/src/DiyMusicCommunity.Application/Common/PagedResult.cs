namespace DiyMusicCommunity.Application.Common;

/// <summary>Generic wrapper for a paginated result set.</summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Items on the current page.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Current page number (1-based).</summary>
    /// <example>1</example>
    public int Page { get; }

    /// <summary>Maximum number of items per page.</summary>
    /// <example>20</example>
    public int PageSize { get; }

    /// <summary>Total number of items matching the filters (before pagination).</summary>
    /// <example>42</example>
    public int TotalCount { get; }

    /// <summary>Initialises a new <see cref="PagedResult{T}"/>.</summary>
    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
