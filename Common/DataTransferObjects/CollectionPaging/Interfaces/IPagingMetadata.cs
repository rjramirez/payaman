namespace Common.DataTransferObjects.CollectionPaging.Interfaces
{
    public interface IPagingMetadata
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int PageSize { get; }
        int TotalCount { get; }
        bool HasPrevious { get; }
        bool HasNext { get; }
        string PageClickEvent { get; set; }
    }
}
