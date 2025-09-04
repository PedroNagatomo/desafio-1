namespace Hypesoft.Application.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }

    public PagedResultDto() { }

    public PagedResultDto(IEnumerable<T> data, int page, int pageSize, long TotalCount)
    {
        Data = data;
        Page = page;
        PageSize = pageSize;
        TotalCount = TotalCount;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
        HasPrevious = page > 1;
        HasNext = page < TotalPages;
    }

}