using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.Pos.StockTransfers;
using RhyoliteOrbit.Models.Pos;

namespace RhyoliteOrbit.DomainServices.Pos.Payments;

public class PaymentManager: DomainService, IPaymentManager
{
    private readonly IRepository<Payment, Guid> _repositoryPayment;

    public PaymentManager(IRepository<Payment, Guid> repositoryPayment)
    {
        _repositoryPayment = repositoryPayment;
    }

        
    public async Task<object> GetAll(int pageNo, int pageSize, string receiptNo, string customerPhoneNo,
        string startDate, string endDate, string paymentSignature)
    {
        
        var predicate = PredicateBuilder.New<Payment>();

        predicate.And(x => !x.IsDeleted);
        
        if (!string.IsNullOrEmpty(receiptNo))
        {
            predicate.And(x => x.ReceiptNo == receiptNo);

        }
        
        if (!string.IsNullOrEmpty(paymentSignature))
        {
            predicate.And(x => x.Id == Guid.Parse(paymentSignature));

        }
        
        if (!string.IsNullOrEmpty(customerPhoneNo))
        {
            predicate.And(x => x.CustomerPhoneNo == customerPhoneNo);

        }
        
        if (!string.IsNullOrEmpty(startDate))
        {
            predicate.And(x => x.CreationTime >= Convert.ToDateTime(startDate));

        }
        
        if (!string.IsNullOrEmpty(endDate))
        {
            predicate.And(x => x.CreationTime <= Convert.ToDateTime(endDate));

        }
        
        var totalCount =
            
            await _repositoryPayment.CountAsync(predicate);
        
        var data = await (await _repositoryPayment.GetAllAsync()).Where(predicate).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

        var paginatedResult = new
        {
            pageNo,
            totalCount,
            totalPages,
            data,
            lowerBound = pageSize * (pageNo - 1) + 1,
            upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
        };

        return paginatedResult;
        
    }
}