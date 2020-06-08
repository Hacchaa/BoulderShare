using System;
using UnityEngine;
using SA.Foundation.Templates;
using SA.Foundation.Time;
using SA.Foundation.Utility;

namespace SA.iOS.StoreKit
{
    /// <inheritdoc cref="ISN_iSKPaymentTransaction" />
    [Serializable]
    class ISN_SKPaymentTransaction : SA_Result, ISN_iSKPaymentTransaction
    {
        [SerializeField]
        string m_ProductIdentifier = null;
        [SerializeField]
        string m_TransactionIdentifier = null;
        [SerializeField]
        long m_UnixDate = 0;
        [SerializeField]
        ISN_SKPaymentTransactionState m_State;
        [SerializeField]
        ISN_SKPaymentOriginalTransaction m_OriginalTransaction = null;

        public ISN_SKPaymentTransaction(ISN_SKProduct product, ISN_SKPaymentTransactionState state)
        {
            m_ProductIdentifier = product.ProductIdentifier;
            m_TransactionIdentifier = SA_IdFactory.RandomString;
            m_UnixDate = SA_Unix_Time.ToUnixTime(DateTime.UtcNow);

            m_State = state;
        }

        public string ProductIdentifier => m_ProductIdentifier;

        public string TransactionIdentifier => m_TransactionIdentifier;

        public DateTime Date => SA_Unix_Time.ToDateTime(m_UnixDate);

        public ISN_SKPaymentTransactionState State => m_State;

        public ISN_SKProduct Product => ISN_SKPaymentQueue.GetProductById(m_ProductIdentifier);

        public ISN_iSKPaymentTransaction OriginalTransaction => m_OriginalTransaction;
    }
}
