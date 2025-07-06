using System.Threading.Tasks;
using RhyoliteOrbit.Models.Payroll;

namespace RhyoliteOrbit.DomainServices.Payroll.IrsSignatures;

public interface IIrsSignatureManager: Abp.Domain.Services.IDomainService
{
    Task Create(IrsSignature entity);
    Task<object> GetSignature();
}