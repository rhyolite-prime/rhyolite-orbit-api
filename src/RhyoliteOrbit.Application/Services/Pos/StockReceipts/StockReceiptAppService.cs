using Abp.Authorization;
using RhyoliteOrbit.DomainServices.Pos.StockReceipts;

namespace RhyoliteOrbit.Services.Pos.StockReceipts;

[AbpAuthorize]
public class StockReceiptAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IStockReceiptManager _stockReceiptManager;

    public StockReceiptAppService(IStockReceiptManager stockReceiptManager)
    {
        _stockReceiptManager = stockReceiptManager;
    }
}