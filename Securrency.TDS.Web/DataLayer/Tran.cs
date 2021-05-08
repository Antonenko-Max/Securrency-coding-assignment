using System.Transactions;

namespace Securrency.TDS.Web.DataLayer
{
    internal static class Tran
    {
        public static TransactionScope BeginScope(IsolationLevel isolationLevel)
        {
            return new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = isolationLevel}, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}