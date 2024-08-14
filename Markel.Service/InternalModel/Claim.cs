using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Markel.Service.InternalModel;

public partial class Claim
{
    public string? Ucr { get; set; }

    public int? CompanyId { get; set; }

    public DateTime? ClaimDate { get; set; }

    public DateTime? LossDate { get; set; }

    public string? AssuredName { get; set; }

    public decimal? IncurredLoss { get; set; }

    public bool? Closed { get; set; }

    public int AgeInDays
    {
        get
        {
            var diff = DateTime.Now - ClaimDate;
            if (diff == null)
            {
                return 0;
            }
            else
            {
                return diff.Value.Days;
            }
        }
    }

    public bool IsNull
    {
        get
        {
            return (Ucr == null && CompanyId == null
                && ClaimDate == null && LossDate == null
                && string.IsNullOrEmpty(AssuredName) && IncurredLoss == null
                && Closed == null);
        }
    }
}
