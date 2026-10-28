using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User
{
    [Authorize]
    public class AssessmentReportModel : PageModel
    {
        private const int defaultPageSize = 10;
        private long userId;
        public int[] PageSizes = { 10, 30, 50, 100, 200 };

        private readonly ILogger<AssessmentInfoModel> _logger;
        private readonly IAssessmentReportRepository _assessmentReportRepository;
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public IEnumerable<StudentAssessmentInfo> StudentAssessmentInfos { get; set; }
        public int CurrentPageNumber { get; set; }
        public int SortByColumnName { get; set; }
        public int SelectedPageSize { get; set; }
        public bool AllowNextPage { get; set; }
        public bool AllowThirdPage { get; set; }
        public int Count { get; set; }
        public string SortByColumnDisplayName { get; set; }
        public AssessmentReportModel(
            ILogger<AssessmentInfoModel> logger,
            IAssessmentReportRepository assessmentReportRepository
            )
        {
            _logger = logger;
            _assessmentReportRepository = assessmentReportRepository;
        }

        public async Task<IActionResult> OnGetAsync(int p, int sc, int ps)
        {
            // p is currentPage
            // sc is sortByColumn
            // ps is page size
            try
            {
                userId = HttpContext.GetUserId();
                if (userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                

                int organizationId = HttpContext.GetOrganizationId();
                if(organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                Count = await _assessmentReportRepository.GetAssessmentCountByOrganizationIdAsyc(organizationId);
                if (!PageSizes.Any(pss => pss == ps))
                {
                    ps = defaultPageSize;
                }
                SelectedPageSize = ps == 0 ? defaultPageSize : ps;
                CurrentPageNumber = p = (p == 0 || (Count + ps - (SelectedPageSize * p) < 0)) ? 1 : p;
                SortByColumnName = sc == 0 ? 1 : sc;
                AllowNextPage = Count - (SelectedPageSize * p) > 0;
                AllowThirdPage = Count - (SelectedPageSize * 3) > 0;
                var sortColumnDatabaseName = GetSortByColumnName(sc);
                var model = new StudentAssessmentReportRequestModel()
                {
                    PageNumber = CurrentPageNumber,
                    PageSize = SelectedPageSize,
                    SortColumn = sortColumnDatabaseName,
                    SortDirection = sortColumnDatabaseName == SortColumns.TimeStamp.ToString() ?
                                                        SortDirection.DESC : SortDirection.ASC
                };

                StudentAssessmentInfos = await _assessmentReportRepository.GetByOrganizationIdAsync(organizationId, model.ToPagingModel());
                SetBreadcrumb();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=AssessmentReportModel.OnGetAsync(), UserId={userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Assessment Reports" }
                },
                IsResponsive = true
            };
        }

        private string GetSortByColumnName(int sortColumn)
        {
            switch (sortColumn)
            {
                case 1:
                    SortByColumnDisplayName = "Assessment";
                    return SortColumns.AssessmentName.ToString();
                case 2:
                    SortByColumnDisplayName = "Class";
                    return SortColumns.ClassName.ToString();
                case 3:
                    SortByColumnDisplayName = "Parent Name";
                    return SortColumns.ParentName.ToString();
                case 4:
                    SortByColumnDisplayName = "Student Name";
                    return SortColumns.StudentName.ToString();
                case 5:
                    SortByColumnDisplayName = "Time Stamp";
                    return SortColumns.TimeStamp.ToString();
                default:
                    SortByColumnDisplayName = "Assessment";
                    return SortColumns.AssessmentName.ToString();
            }
        }
    }
}
