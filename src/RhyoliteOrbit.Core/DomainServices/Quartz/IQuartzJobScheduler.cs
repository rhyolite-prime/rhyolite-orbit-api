using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.DomainServices.Quartz.Dto;

namespace RhyoliteOrbit.DomainServices.Quartz;

public interface IQuartzJobScheduler: IDomainService
{
    Task<object> CreateJob(JobModel jobModel);
    
    Task<bool> DeleteJob(string jobName);
}