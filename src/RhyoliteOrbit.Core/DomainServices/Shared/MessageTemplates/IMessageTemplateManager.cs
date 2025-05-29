using System;
using System.Threading.Tasks;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.Shared.MessageTemplates;

public interface IMessageTemplateManager : Abp.Domain.Services.IDomainService
{
    Task<object> ListAll(int tenantId, int creationTime, string name);

     Task Create(MessageTemplate input);
    
    Task Update(MessageTemplate input);
    
    Task Delete(Guid id);
}