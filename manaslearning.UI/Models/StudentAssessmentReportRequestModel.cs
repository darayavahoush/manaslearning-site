using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Models
{
    public enum SortColumns
    {
        AssessmentName = 1,
        ClassName,
        ParentName,
        StudentName,
        TimeStamp
    }

    public class StudentAssessmentReportRequestModel : PagingRequestModel
    {
        public Paging ToPagingModel()
        {
            Paging paging = new Paging();
            paging.PageNumber = PageNumber;
            paging.PageSize = PageSize;
            paging.SearchInput = SearchInput;
            if (string.IsNullOrEmpty(SortColumn))
            {
                paging.SortDirection = null;
                paging.SortColumn = null;
            }
            else
            {
                paging.SortDirection = (short)SortDirection;
                paging.SortColumn = GetSortMapping()[SortColumn];
            }
            return paging;
        }

        public Dictionary<string, int> GetSortMapping()
        {
            Dictionary<string, int> result = new Dictionary<string, int>()
            {
                { SortColumns.AssessmentName.ToString(), 1},
                { SortColumns.ClassName.ToString(), 2 },
                { SortColumns.ParentName.ToString(), 3 },
                { SortColumns.StudentName.ToString(), 4 },
                { SortColumns.TimeStamp.ToString(), 5 }
            };
            return result;
        }

        /// <summary>
        /// Determines StudentAssessmentReportRequestModel model is valid.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        public bool IsValid()
        {
            if (!string.IsNullOrEmpty(SearchInput) && SearchInput.Any(c => !Char.IsLetterOrDigit(c)))
            {
                return false;
            }
            return true;
        }
    }
}
