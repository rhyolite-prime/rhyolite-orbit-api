using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Shared.MetaData;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SchoolProfiles;

public class SchoolProfileManager: Abp.Domain.Services.DomainService, ISchoolProfileManager
{
        private readonly IRepository<SchoolProfile, Guid> _repositorySchoolProfile;
        private readonly IRepository<SystemSetting, Guid> _repositorySystemSetting;
        private readonly IAcademicPeriodMetaDataManager _academicPeriodMetaDataManager;
       
        public SchoolProfileManager(IRepository<SchoolProfile, Guid> repositorySchoolProfile, IAcademicPeriodMetaDataManager academicPeriodMetaDataManager, IRepository<SystemSetting, Guid> repositorySystemSetting)
        {
            _repositorySchoolProfile = repositorySchoolProfile;
            _academicPeriodMetaDataManager = academicPeriodMetaDataManager;
            _repositorySystemSetting = repositorySystemSetting;
        }

        public async Task Create(SchoolProfile entity)
        {
            var academicYearInfo = await _academicPeriodMetaDataManager.GetAcademicPeriodMeta(entity.CurrentAcademicYearId, entity.CurrentTermId);
            
            var datta = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();

            if (datta != null)
            {
                datta.SchoolName = entity.SchoolName;
                datta.CurrentAcademicYearId = entity.CurrentAcademicYearId;
                datta.CurrentAcademicYearName = academicYearInfo.AcademicYearName;
                datta.CurrentTermId = entity.CurrentTermId;
                datta.CurrentTermName = academicYearInfo.TermName;
                datta.SecondaryEmailAddress = entity.SecondaryEmailAddress;
                datta.City = entity.City;
                datta.PrimaryPhoneNo = entity.PrimaryPhoneNo;
                datta.Website = entity.Website;
                datta.PostalAddress = entity.PostalAddress;
                datta.District = entity.District;
                datta.SecondaryPhoneNo = entity.SecondaryPhoneNo;
                datta.SchoolHead = entity.SchoolHead;
                datta.SchoolLogoUrl = !string.IsNullOrEmpty(entity.SchoolLogoUrl) ? entity.SchoolLogoUrl : "";
                datta.RegionOrState = entity.RegionOrState;
                datta.AssistantSchoolHead = entity.AssistantSchoolHead;

                await _repositorySchoolProfile.UpdateAsync(datta);

            }
            else
            {
                entity.CurrentAcademicYearName = academicYearInfo.AcademicYearName;
                entity.CurrentTermName = academicYearInfo.TermName;

                await _repositorySchoolProfile.InsertAsync(entity);
            }

        }

        public async Task<SchoolProfile> GetAsync()
        {
            var data = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();
            return data;
        }

        public async Task CreateReportTemplate(string fileUrl, string reportType)
        {
            var systemSettings = await (await _repositorySystemSetting.GetAllAsync()).FirstOrDefaultAsync();
            
            if (systemSettings != null) {

                switch (reportType)
                {
                    case "bill":
                        systemSettings.BillPaymentEmailTemplateHtmlUrl = fileUrl;
                        break;
                    case "terminal-report":
                        systemSettings.TerminalReportTemplateHtmlUrl = fileUrl;
                        break;
                    default:
                        break;
                }

                await _repositorySystemSetting.UpdateAsync(systemSettings);
            }
        }
}