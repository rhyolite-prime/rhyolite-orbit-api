using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.UomCodes;

public interface IUomCodeManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(UomCode input);
    Task Update(UomCode input);
    Task Delete(Guid id);
}