using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.CreditTerms;

public interface ICreditTermManager: IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task Create(CreditTerm input);
    Task Update(CreditTerm input);
    Task Delete(Guid id);
}