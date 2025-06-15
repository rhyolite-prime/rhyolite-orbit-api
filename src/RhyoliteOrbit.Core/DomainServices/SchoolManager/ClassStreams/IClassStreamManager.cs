using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.ClassStreams;

public interface IClassStreamManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> Create(ClassStream input);
    Task Update(ClassStream input);
    Task Delete(Guid id);
}