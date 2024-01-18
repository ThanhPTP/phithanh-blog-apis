namespace PhiThanh.Core
{
    public class PagingFilterOption
    {
        public PagingFilterOption()
        {
            PageIndex = 1;
            PageSize = 50;
        }

        /// <summary>
        /// Default pageSize = 10
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Default pageIndex = 1
        /// </summary>
        /// </summary>
        public int PageIndex { get; set; }

        public Dictionary<string, string> Sorter { get; set; } = [];
    }
}
