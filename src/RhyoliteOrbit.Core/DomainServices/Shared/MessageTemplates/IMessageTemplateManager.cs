using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.MessageTemplates;

public interface IMessageTemplateManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int pageNo, int pageSize, string query);

     Task Create(MessageTemplate input);
    
    Task Update(MessageTemplate input);
    
    Task Delete(Guid id);
}