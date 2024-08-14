using INT = Markel.Service.InternalModel;
using EXT = Markel.Api.ExternalModel;

namespace Markel.Api.Mapping
{
    public static class CompanyMapping
    {
        public static EXT.Company MapCompanyToExternal(INT.Company company)
        {
            return new EXT.Company
            {
                Id = company.Id,
                Name = company.Name,
                Postcode = company.Postcode,
                Address1 = company.Address1,
                Address2 = company.Address2,
                Address3 = company.Address3,
                Active = company.Active,
                Country = company.Country,
                InsuranceEndDate = company.InsuranceEndDate
            };
        }
    }
}
