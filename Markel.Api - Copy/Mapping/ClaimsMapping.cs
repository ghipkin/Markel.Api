using INT = Markel.Service.InternalModel;
using EXT = Markel.Api.ExternalModel;

namespace Markel.Api.Mapping
{
    public static class ClaimsMapping
    {
        public static IEnumerable<EXT.Claim> MapClaimsToExternal(IEnumerable<INT.Claim> claims)
        { 
            var result = new List<EXT.Claim>();

            foreach (var claim in claims) 
            { 
                result.Add(MapClaimToExternal(claim));
            }

            return result;
        }

        public static EXT.Claim MapClaimToExternal(INT.Claim claim)
        {
            return new EXT.Claim
            {
                AssuredName = claim.AssuredName,
                ClaimDate = claim.ClaimDate,
                Closed = claim.Closed,
                CompanyId = claim.CompanyId,
                IncurredLoss = claim.IncurredLoss,
                LossDate = claim.LossDate,
                Ucr = claim.Ucr,
                AgeInDays = claim.AgeInDays,
            };
        }

        public static INT.Claim MapClaimToInternal(EXT.Claim claim)
        {
            return new INT.Claim
            {
                AssuredName = claim.AssuredName,
                ClaimDate = claim.ClaimDate,
                Closed = claim.Closed,
                CompanyId = claim.CompanyId,
                IncurredLoss = claim.IncurredLoss,
                LossDate = claim.LossDate,
                Ucr = claim.Ucr,
            };
        }
    }
}
