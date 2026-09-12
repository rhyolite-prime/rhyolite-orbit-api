using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Deliveries;

public class DeliveryManager: DomainService, IDeliveryManager
{
    private readonly IRepository<Delivery, Guid> _deliveryRepository;

    public DeliveryManager(IRepository<Delivery, Guid> deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    #region Lifecycle & Creation

    public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
    {
        if (delivery.OrderId == Guid.Empty)
        {
            throw new UserFriendlyException(400, "Delivery must be linked to a valid POS Order.");
        }

        if (string.IsNullOrWhiteSpace(delivery.RecipientPhone))
        {
            throw new UserFriendlyException(400, "Recipient phone number is required.");
        }

        // Generate tracking number and random 6-digit confirmation OTP
        if (string.IsNullOrWhiteSpace(delivery.TrackingNumber))
        {
            delivery.TrackingNumber = $"DEL-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        }

        delivery.DeliveryConfirmationCode = Random.Shared.Next(100000, 999999).ToString();
        delivery.Status = RhyoliteOrbitConsts.DeliveryStatus.Pending;

        return await _deliveryRepository.InsertAsync(delivery);
    }

    public async Task CancelDeliveryAsync(Guid deliveryId, string reason)
    {
        var delivery = await GetDeliveryByIdAsync(deliveryId);

        if (delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Delivered)
        {
            throw new UserFriendlyException(400, "Cannot cancel a delivery that has already been fulfilled.");
        }

        delivery.Status = RhyoliteOrbitConsts.DeliveryStatus.Cancelled;
        delivery.ExtraNotes = string.IsNullOrWhiteSpace(delivery.ExtraNotes)
            ? $"Cancelled: {reason}"
            : $"{delivery.ExtraNotes} | Cancelled: {reason}";

        await _deliveryRepository.UpdateAsync(delivery);
    }

    #endregion

    #region Dispatch & Courier Management

    public async Task AssignRiderAsync(Guid deliveryId, Guid riderId, string riderName, string riderPhone)
    {
        var delivery = await GetDeliveryByIdAsync(deliveryId);

        if (delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Delivered || delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Cancelled)
        {
            throw new UserFriendlyException(400, $"Cannot assign rider to a delivery with status '{delivery.Status}'.");
        }

        delivery.RiderId = riderId;
        delivery.RiderName = riderName;
        delivery.RiderPhone = riderPhone;
        delivery.Status = RhyoliteOrbitConsts.DeliveryStatus.Assigned;

        await _deliveryRepository.UpdateAsync(delivery);
    }

    public async Task MarkAsDispatchedAsync(Guid deliveryId, DateTime? estimatedDeliveryTime = null)
    {
        var delivery = await GetDeliveryByIdAsync(deliveryId);

        if (!delivery.RiderId.HasValue)
        {
            throw new UserFriendlyException(400, "A rider must be assigned before dispatching a delivery.");
        }

        if (delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Delivered || delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Cancelled)
        {
            throw new UserFriendlyException(400, "Cannot dispatch a completed or cancelled delivery.");
        }

        delivery.Status = RhyoliteOrbitConsts.DeliveryStatus.InTransit;
        delivery.DispatchedTime = DateTime.UtcNow;
        delivery.EstimatedDeliveryTime = estimatedDeliveryTime ?? DateTime.UtcNow.AddHours(2);

        await _deliveryRepository.UpdateAsync(delivery);
    }

    #endregion

    #region Tracking & Verification

    public async Task UpdateDeliveryStatusAsync(Guid deliveryId, RhyoliteOrbitConsts.DeliveryStatus status, string notes = null)
    {
        var delivery = await GetDeliveryByIdAsync(deliveryId);

        if (delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Delivered)
        {
            throw new UserFriendlyException(400, "Delivery has already been marked as delivered.");
        }

        delivery.Status = status;

        if (!string.IsNullOrWhiteSpace(notes))
        {
            delivery.ExtraNotes = string.IsNullOrWhiteSpace(delivery.ExtraNotes)
                ? notes
                : $"{delivery.ExtraNotes} | {notes}";
        }

        await _deliveryRepository.UpdateAsync(delivery);
    }

    public async Task<bool> ConfirmDeliveryOtpAsync(Guid deliveryId, string confirmationCode)
    {
        var delivery = await GetDeliveryByIdAsync(deliveryId);

        if (delivery.Status == RhyoliteOrbitConsts.DeliveryStatus.Delivered)
        {
            throw new UserFriendlyException(400, "Delivery is already marked as completed.");
        }

        if (string.IsNullOrWhiteSpace(confirmationCode) || delivery.DeliveryConfirmationCode != confirmationCode.Trim())
        {
            throw new UserFriendlyException(400, "Invalid delivery confirmation OTP code.");
        }

        // Finalize delivery
        delivery.Status = RhyoliteOrbitConsts.DeliveryStatus.Delivered;
        delivery.DeliveredTime = DateTime.UtcNow;

        await _deliveryRepository.UpdateAsync(delivery);
        return true;
    }

    #endregion

    #region Queries

    public async Task<Delivery> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _deliveryRepository.FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber);
    }

    public async Task<List<Delivery>> GetActiveDeliveriesForRiderAsync(Guid riderId)
    {
        return await _deliveryRepository.GetAllListAsync(
            x => x.RiderId == riderId &&
                (x.Status == RhyoliteOrbitConsts.DeliveryStatus.Assigned || x.Status == RhyoliteOrbitConsts.DeliveryStatus.PickedUp || x.Status == RhyoliteOrbitConsts.DeliveryStatus.InTransit)
        );
    }

    #endregion

    #region Private Helpers

    private async Task<Delivery> GetDeliveryByIdAsync(Guid deliveryId)
    {
        var delivery = await _deliveryRepository.FirstOrDefaultAsync(deliveryId);
        if (delivery == null)
        {
            throw new UserFriendlyException(404, "Delivery record not found.");
        }
        return delivery;
    }

    #endregion
}