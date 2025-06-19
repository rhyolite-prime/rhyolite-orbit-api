using System;

namespace RhyoliteOrbit.Models.Shared;

public class CompanyProfile: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    public int TenantId { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string RegionOrState { get; set; }
        public string CompanyTin { get; set; }
        public string Email { get; set; }
        public int FirstMonthOfFiscalYear { get; set; }
        public string VatNo { get; set; }

        //Social Security Rates...
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
        //email settings
        public string MailHostName { get; set; }
        public bool IsSsLEnabled { get; set; }
        public int PortNumber { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string EmailPassword { get; set; }
        
        //tax levies inclusive
        public bool IsTaxLeviesInclusive { get; set; }
}