namespace Shared.SeedWork
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
        {
            _metaData = new MetaData
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double) pageSize)
            };

            AddRange(items);
        }

        private MetaData _metaData { get; set; }

        public MetaData GetMeta()
        {
            return _metaData;   
        }
    }
}
