using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.CrossPlatform.InApp
{
    internal static class UM_AndroidInAppTransactions 
    {

       [Serializable]
        public class TransactionsList
        {
            public List<string> Completed = new List<string>();
        }

        private const string k_CompletedTransactionsListKey = "COMPLETED_TRANSACTIONS_LIST_KEY";
        private static TransactionsList m_TransactionsList;
        
        public static void RegisterCompleteTransaction(string id) 
        {
            Transactions.Completed.Add(id);
            Save();
        }

        public static bool IsTransactionCompleted(string id) 
        {
            return Transactions.Completed.Contains(id);
        }
        
        private static void Save()
        {
            var json = JsonUtility.ToJson(Transactions);
            PlayerPrefs.SetString(k_CompletedTransactionsListKey, json);
            PlayerPrefs.Save();
        }
        
        private static TransactionsList Transactions 
        {
            get {
                if(m_TransactionsList == null)
                {
                    if (PlayerPrefs.HasKey(k_CompletedTransactionsListKey)) 
                    {
                        var json = PlayerPrefs.GetString(k_CompletedTransactionsListKey);
                        m_TransactionsList = JsonUtility.FromJson<TransactionsList>(json);
                    } 
                    else 
                    {
                        m_TransactionsList = new TransactionsList();
                    }
                }
  
                return m_TransactionsList;
            }
        }
    }
}