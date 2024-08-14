using DL = Markel.DataLayer;
using INT = Markel.Service.InternalModel;
using Markel.Service.Mapping;
using Markel.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Markel.Service
{
    public class MarkelService : IMarkelService
    {
        internal const string ERR_FAILED_TO_SAVE_CLAIM = "Failed to save claim.";
        private readonly DL.MarkelContext _dbContext;
        public MarkelService(DL.MarkelContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public async Task<INT.Company?> GetCompany(int companyId)
        {
            DL.Company? company = _dbContext.Companies.SingleOrDefault(x=>x.Id == companyId);

            if (company == null)
            {
                return null;
            }

            var companyclaims = await _dbContext.Claims.Where(x=>x.CompanyId == companyId && x.Closed==false)
                                .ToListAsync();

            bool hasClaims = (companyclaims != null && companyclaims.Any());
            return CompanyMapping.MapCompanyToInternal(company,hasClaims);
        }

        public async Task<IEnumerable<INT.Claim>> GetClaims(int companyid)
        {
            IEnumerable<DL.Claim> claims = await _dbContext.Claims.Where(x=>x.CompanyId == companyid)
                                                    .ToListAsync();

            return ClaimsMapping.MapClaimsToInternal(claims);
        }

        public async Task<INT.Claim?> GetClaim(string claimRef)
        {
            DL.Claim? claim = await _dbContext.Claims.SingleOrDefaultAsync(x => x.Ucr == claimRef);

            if (claim == null)
            {
                return null;
            }
            else
            {
                return ClaimsMapping.MapClaimToInternal(claim);
            }
        }

        public async Task UpdateClaim(INT.Claim claim)
        {
            
            DL.Claim? claimToUpdate = await _dbContext.Claims.SingleOrDefaultAsync(x => x.Ucr == claim.Ucr);

            if (claimToUpdate == null) 
            {
                throw new ClaimNotFoundException();
            }

            claimToUpdate.AssuredName = claim.AssuredName;
            claimToUpdate.ClaimDate = claim.ClaimDate;
            claimToUpdate.LossDate = claim.LossDate;
            claimToUpdate.CompanyId = claim.CompanyId;
            claimToUpdate.Closed = claim.Closed;
            claimToUpdate.IncurredLoss = claim.IncurredLoss;

            var transaction = _dbContext.Database.BeginTransaction();
            int rowsAffected = await _dbContext.SaveChangesAsync();

            if (rowsAffected != 1)
            {
                transaction.Rollback();
                throw new Exception(ERR_FAILED_TO_SAVE_CLAIM);
            }
            transaction.Commit();
        }
    }
}
