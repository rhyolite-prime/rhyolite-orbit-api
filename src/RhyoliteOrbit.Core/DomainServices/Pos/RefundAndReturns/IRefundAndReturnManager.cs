using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.RefundAndReturns;

public interface IRefundAndReturnManager: IDomainService
{
    // Request Lifecycle
    Task<RefundAndReturn> CreateRequestAsync(RefundAndReturn request);
    Task ApproveRequestAsync(Guid refundRequestId, long approvedByUserId, string adminNotes = null);
    Task RejectRequestAsync(Guid refundRequestId, long rejectedByUserId, string reason);
    Task ProcessRefundAsync(Guid refundRequestId, long processedByUserId);

    // Validation & Queries
    Task<bool> ValidateReturnEligibilityAsync(Guid orderId, List<RefundItemDetail> itemsToReturn);
    Task<List<RefundAndReturn>> GetPendingRequestsByBranchAsync(Guid branchId);
    Task<List<RefundAndReturn>> GetRequestsByCustomerAsync(Guid customerId);
}