
using System;
using INT = Markel.Service.InternalModel;
using DL = Markel.DataLayer
;

namespace Markel.Service.Mapping
{
    public static class CompanyMapping
    {
        public static INT.Company MapCompanyToInternal(DL.Company company, bool hasClaims)
        {
            return new INT.Company
            {
                Id = company.Id,
                Name = company.Name,
                Postcode = company.Postcode,
                Address1 = company.Address1,
                Address2 = company.Address2,
                Address3 = company.Address3,
                Active = company.Active,
                Country = company.Country,
                InsuranceEndDate = company.InsuranceEndDate,
                HasActiveClaim= hasClaims
            };
        }
    }
}
