using SA.Foundation.Templates;

namespace SA.CrossPlatform.InApp
{
    public interface UM_iTransactionObserver
    {
        void OnTransactionUpdated(UM_iTransaction transaction);
        void OnRestoreTransactionsComplete(SA_Result result);
    }
}