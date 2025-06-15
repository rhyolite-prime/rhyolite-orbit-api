using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.PastQuestions;

public interface IPastQuestionManager: Abp.Domain.Services.IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string query);
    Task<object> Create(PastQuestion input);
    Task Update(PastQuestion input);
    Task Delete(Guid id);
}