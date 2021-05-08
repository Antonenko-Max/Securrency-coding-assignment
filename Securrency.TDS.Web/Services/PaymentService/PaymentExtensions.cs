using Securrency.TDS.Web.DataLayer.Entities;
using stellar_dotnet_sdk.responses.operations;

namespace Securrency.TDS.Web.Services.PaymentService
{
    public static class PaymentExtensions
    {
        public static PaymentEntity ToEntity(this PaymentOperationResponse model)
        {
            return new PaymentEntity()
            {
                Id = model.Id,
                SourceAccountId = model.SourceAccount,
                TransactionSuccessful = model.TransactionSuccessful,
                To = model.To,
                From = model.From,
                Amount = decimal.Parse(model.Amount)
            };
        }
    }
}
