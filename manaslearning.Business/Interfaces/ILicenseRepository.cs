using manaslearning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace manaslearning.Business.Interfaces
{
    public interface ILicenseRepository
    {
        Task<IEnumerable<License>> GetOrganizationLicensesAsync(int organizationId);

        Task<License> GetOrganizationLicenseByAssessmentIdAsync(int organizationId, int assessmentId);

        Task<IEnumerable<LicenseInfo>> GetOrganizationLicensesInfoAsync(int organizationId);

        Task<IEnumerable<LicenseInfo>> GetLicensesInfoByOrganizationOidAsync(Guid organizationOid);

        Task<int> ReduceAvailableLicenseByOne(int organizationId, int assessmentId);

        Task<int> AddOrganizationLicensesAsync(int organizationId, int assessmentId, int itemId, int licenses, long createdBy);

        Task<int> ReduceOrganizationLicensesAsync(int organizationId, int assessmentId, int licenses, long createdBy);
    }
}
