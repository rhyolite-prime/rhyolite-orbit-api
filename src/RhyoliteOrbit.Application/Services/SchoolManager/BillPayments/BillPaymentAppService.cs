using RhyoliteOrbit.DomainServices.SchoolManager.BillPayments;

namespace RhyoliteOrbit.Services.SchoolManager.BillPayments;

public class BillPaymentAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IBillPaymentManager  _billPaymentManager;

    public BillPaymentAppService(IBillPaymentManager billPaymentManager)
    {
        _billPaymentManager = billPaymentManager;
    }
    
    
    
}