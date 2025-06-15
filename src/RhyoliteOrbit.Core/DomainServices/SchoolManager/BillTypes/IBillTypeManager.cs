using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.BillTypes;

public interface IBillTypeManager: Abp.Domain.Services.IDomainService
{
    Task<object> ListAll();
    Task<object> Create(BillType input);
    Task Update(BillType input);
    Task Delete(Guid id);
}