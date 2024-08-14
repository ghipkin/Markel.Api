using System;
using System.Collections.Generic;

namespace Markel.Service.InternalModel;

public partial class Company
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Address3 { get; set; }

    public string? Postcode { get; set; }

    public string? Country { get; set; }

    public bool? Active { get; set; }

    public DateTime? InsuranceEndDate { get; set; }

    public bool HasActiveClaim { get; set; }
    public bool IsNull
    {
        get 
        {
            return (Id == null
                && string.IsNullOrEmpty(Address1) && string.IsNullOrEmpty(Address2) && string.IsNullOrEmpty(Address3)
                && string.IsNullOrEmpty(Postcode) && string.IsNullOrEmpty(Country) && string.IsNullOrEmpty(Name)
                && Active == null && InsuranceEndDate == null);
        }
    }
}
