using Turbo.Shared.Application.ResponseObject.Enums;

namespace Turbo.Shared.Application.ResponseObject.Concreate;

public class PagedResponse<T> : Response<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PagedResponse(T data, int pageIndex, int pageSize, int totalCount)
        : base(ResponseStatusCode.Success, data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public PagedResponse(T data, int pageIndex, int pageSize, int totalCount, string message)
        : base(ResponseStatusCode.Success, data, message)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    // Fluent API methods
    public new PagedResponse<T> WithMessage(string message)
    {
        Message = message;
        return this;
    }

    public new PagedResponse<T> WithMetadata(string key, object value)
    {
        Metadata[key] = value;
        return this;
    }

    public new PagedResponse<T> WithMetadata(Dictionary<string, object> metadata)
    {
        Metadata = metadata;
        return this;
    }

    // Static factory methods
    public static PagedResponse<T> Create(T data, int pageIndex, int pageSize, int totalCount) =>
        new(data, pageIndex, pageSize, totalCount);

    public static PagedResponse<T> Create(
        T data,
        int pageIndex,
        int pageSize,
        int totalCount,
        string message
    ) => new(data, pageIndex, pageSize, totalCount, message);

    public static PagedResponse<T> CreateWithMetadata(
        T data,
        int pageIndex,
        int pageSize,
        int totalCount,
        Dictionary<string, object> metadata
    )
    {
        var response = new PagedResponse<T>(data, pageIndex, pageSize, totalCount);
        response.Metadata = metadata;
        return response;
    }
}
