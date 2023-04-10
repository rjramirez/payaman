using Common.DataTransferObjects.CollectionPaging.Interfaces;
using Newtonsoft.Json;

namespace Common.DataTransferObjects.CollectionPaging
{
    public class PagedList<TEntity> : List<TEntity>, IPagingMetadata
    {
        public PagedList()
        {

        }

        public PagedList(List<TEntity> items, PagingMetadata pagingMetadata)
        {
            CurrentPage = pagingMetadata.CurrentPage;
            TotalCount = pagingMetadata.TotalCount;
            PageSize = pagingMetadata.PageSize == 0 ? TotalCount : pagingMetadata.PageSize;
            TotalPages = pagingMetadata.PageSize == 0 ? 1 : (int)Math.Ceiling(pagingMetadata.TotalCount / (double)pagingMetadata.PageSize);
            HasPrevious = CurrentPage > 1;
            HasNext = CurrentPage < TotalPages;
            PagingMetadata pagingMetaData = new PagingMetadata(CurrentPage, TotalPages, PageSize, TotalCount, null);

            PagingHeaderValue = JsonConvert.SerializeObject(pagingMetaData);

            AddRange(items);
        }

        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious { get; private set; }

        public bool HasNext { get; private set; }

        public string PageClickEvent { get; set; }

        public string PagingHeaderValue { get; set; }

        public static PagedList<TEntity> ToPagedList(IQueryable<TEntity> source, int pageNumber, int pageSize)
        {
            int count;
            List<TEntity> items;

            if (pageSize != 0)
            {
                count = source.Count();
                items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                items = source.ToList();
                count = items.Count;
            }

            return new PagedList<TEntity>(items, new PagingMetadata(count, pageNumber, pageSize));
        }
    }
}
