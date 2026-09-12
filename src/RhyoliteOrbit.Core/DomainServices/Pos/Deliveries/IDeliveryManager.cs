using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Deliveries;

public interface IDeliveryManager: IDomainService
{
    Task<Delivery> CreateDeliveryAsync(Delivery delivery);
    Task CancelDeliveryAsync(Guid deliveryId, string reason);

    // Dispatch & Courier Management
    Task AssignRiderAsync(Guid deliveryId, Guid riderId, string riderName, string riderPhone);
    Task MarkAsDispatchedAsync(Guid deliveryId, DateTime? estimatedDeliveryTime = null);

    // Tracking & Verification
    Task UpdateDeliveryStatusAsync(Guid deliveryId, RhyoliteOrbitConsts.DeliveryStatus status, string notes = null);
    Task<bool> ConfirmDeliveryOtpAsync(Guid deliveryId, string confirmationCode);

    // Queries
    Task<Delivery> GetByTrackingNumberAsync(string trackingNumber);
    Task<List<Delivery>> GetActiveDeliveriesForRiderAsync(Guid riderId);
}