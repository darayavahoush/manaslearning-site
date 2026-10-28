namespace manaslearning.Models
{
    public class Paging
    {

        /// <summary>
        /// Page number 
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Page size for the records
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Sort Column
        /// </summary>
        public int? SortColumn { get; set; }

        /// <summary>
        /// Sort Direction
        /// </summary>
        public short? SortDirection { get; set; }

        /// <summary>
        /// Search Input
        /// </summary>
        public string SearchInput { get; set; }
    }
}

