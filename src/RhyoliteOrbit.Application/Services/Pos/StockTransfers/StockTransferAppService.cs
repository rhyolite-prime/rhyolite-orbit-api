using Abp.Authorization;
using RhyoliteOrbit.DomainServices.Pos.StockTransfers;

namespace RhyoliteOrbit.Services.Pos.StockTransfers;

[AbpAuthorize]
public class StockTransferAppService: RhyoliteOrbitAppServiceBase
{
    private readonly IStockTransferManager _stockTransferManager;

    public StockTransferAppService(IStockTransferManager stockTransferManager)
    {
        _stockTransferManager = stockTransferManager;
    }
}