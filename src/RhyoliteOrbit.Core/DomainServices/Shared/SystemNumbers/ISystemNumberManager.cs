using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.SystemNumbers;

public interface ISystemNumberManager: IDomainService
{
    Task<object> ListAll(string key);
    Task Create(List<SystemNumber> systemNumbers);
    Task Update(SystemNumber input);
}