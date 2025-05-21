using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Shared;

public class BusinessProfile: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    [MaxLength(200)]
    public string CompanyName { get; set; }
    [MaxLength(100)]
    public string Country { get; set; }
    [MaxLength(100)]
    public string CityName { get; set; }
    [MaxLength(50)]
    public string PhoneNo { get; set; }
    [MaxLength(250)]
    public string Address { get; set; }
    [MaxLength(100)]
    public string RegionOrState { get; set; }
    [MaxLength(100)]
    public string CompanyTin { get; set; }
    [MaxLength(150)]
    public string Email { get; set; }
         
    public int FirstMonthOfFiscalYear { get; set; }
    [MaxLength(100)]
    public string VatNo { get; set; }

    //Social Security Rates...
    [MaxLength(100)]
    public string EmployerSocialSecurityNo { get; set; }
    public bool PaySocialSecurityFund { get; set; }
    public decimal SocialSecurityFundEmployeeRate { get; set; }
    public decimal SocialSecurityFundEmployerRate { get; set; }

    //Provident Fund Rates...
    public bool PayProvidentFund { get; set; }
    public decimal ProvidentFundEmployeeRate { get; set; }
    public decimal ProvidentFundEmployerRate { get; set; }
    public bool ProvidentFundTreatAsSocialSecurityFund { get; set; }
    public bool ProvidentFundTaxEmployerPortion { get; set; }

    // 2nd provident fund
    public bool PaySecondProvidentOrOthers { get; set; }
    public decimal SecondProvidentEmployeeRate { get; set; }
    public decimal SecondProvidentEmployerRate { get; set; }
    public bool TreatSecondProvidentAsSsf { get; set; }
    public bool SecondProvidentTaxEmployerPortion { get; set; }

    //Overtime Percentage...
    public bool OvertimePercentageOnBasicSalary { get; set; }
    public bool OvertimePercentageOnDailyWage { get; set; }

    public Guid DefaultCurrencyId { get; set; }
    public int TenantId { get; set; }
}