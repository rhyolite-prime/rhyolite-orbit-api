using System;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.NotificationAccounts;

public interface INotificationAccountManager:IDomainService
{
    Task<object> GetAll(int tenantId);
    Task Create(NotificationAccount input);
    Task Update(NotificationAccount input);
    Task Delete(Guid id);
}