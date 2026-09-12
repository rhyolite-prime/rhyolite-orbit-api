using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.DomainServices.Pos.Payments;

public interface IPaymentManager:  IDomainService
{
    Task<object> GetAll(int pageNo, int pageSize, string receiptNo, string customerPhoneNo, string startDate, string endDate, string itemId);
}