using System;
using INT = Markel.Service.InternalModel;
using DL = Markel.DataLayer;

namespace Markel.Service.Mapping
{
    public static class ClaimsMapping
    {
        public static IEnumerable<INT.Claim> MapClaimsToInternal(IEnumerable<DL.Claim> claims)
        {
            var result = new List<INT.Claim>();

            foreach (var claim in claims)
            {
                result.Add(MapClaimToInternal(claim));
            }
            return result;
        }

        public static INT.Claim MapClaimToInternal(DL.Claim claim)
        {
            return new INT.Claim
            {
                AssuredName = claim.AssuredName,
                ClaimDate = claim.ClaimDate,
                Closed = claim.Closed,
                CompanyId = claim.CompanyId,
                IncurredLoss = claim.IncurredLoss,
                LossDate = claim.LossDate,
                Ucr = claim.Ucr
            };
        }

        public static DL. Claim MapClaimToDataLayer(INT.Claim claim)
        {
            return new DL.Claim
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
