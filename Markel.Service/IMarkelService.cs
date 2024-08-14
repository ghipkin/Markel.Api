using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INT = Markel.Service.InternalModel;

namespace Markel.Service
{
    public interface IMarkelService
    {
        public Task<INT.Company?> GetCompany(int companyId);
        public Task<IEnumerable<INT.Claim>> GetClaims(int companyid);
        public Task<INT.Claim?> GetClaim(string claimRef);
        public Task UpdateClaim(INT.Claim claim);
    }
}
