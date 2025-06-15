using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.SchoolProfiles;

public interface ISchoolProfileManager: Abp.Domain.Services.IDomainService
{
    Task<SchoolProfile> GetAsync();
    Task Create(SchoolProfile entity);
    Task CreateReportTemplate(string fileUrl, string reportType);
}