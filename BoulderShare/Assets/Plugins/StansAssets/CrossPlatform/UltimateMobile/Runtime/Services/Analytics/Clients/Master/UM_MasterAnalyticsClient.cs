using UnityEngine;
using System.Collections.Generic;
using SA.Facebook;

namespace SA.CrossPlatform.Analytics
{
    public class UM_MasterAnalyticsClient : UM_iAnalyticsClient
    {
        private bool m_isInitialized = false;
        private List<UM_iAnalyticsInternalClient> m_Clients = null;
        
        public void Init() {

            if (m_isInitialized) {
                Debug.LogError("Client was already Initialized. Make sure you init analytics only once on app start");
                return;
            }

            //All client implemented in a safe manner, so in case client service is missing
            //client will not do anything. 
            m_Clients = new List<UM_iAnalyticsInternalClient>();
            m_Clients.Add(new UM_FirebaseAnalyticsClient());
            m_Clients.Add(new UM_UnityAnalyticsClient());

            if (SA_FB.IsSDKInstalled)
            {
                m_Clients.Add(new UM_FacebookAnalyticsClient());
            }

            m_isInitialized = true;
        }
        
        public void Event(string eventName) 
        {
            if(!m_isInitialized) 
            {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients)
                client.Event(eventName);
        }

        public void Event(string eventName, IDictionary<string, object> data) 
        {
            if (!m_isInitialized) 
            {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients)
                client.Event(eventName, data);
        }


        public void SetUserBirthYear(int birthYear) 
        {

            if (!m_isInitialized) 
            {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients)
                client.SetUserBirthYear(birthYear);
        }

        public void SetUserGender(UM_Gender gender) 
        {

            if (!m_isInitialized) 
            {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients) 
                client.SetUserGender(gender);
        }

        public void SetUserId(string userId) 
        {
            if (!m_isInitialized) {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients) 
                client.SetUserId(userId);
        }

        public void Transaction(string productId, float amount, string currency) 
        {

            if (!m_isInitialized) 
            {
                Debug.LogError("Analytics client has to be initialized prior using any other methods.");
                return;
            }

            foreach (var client in m_Clients) 
                client.Transaction(productId, amount, currency);
        }
        
        public bool IsInitialized 
        {
            get { return m_isInitialized; }
        }
    }
}