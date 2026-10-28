using manaslearning.Models;

namespace manaslearning.UI.Models
{
    public class PagingRequestModel
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
        public string SortColumn { get; set; }

        /// <summary>
        /// Sort Direction
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// Search Input
        /// </summary>
        public string SearchInput { get; set; }
    }
}
