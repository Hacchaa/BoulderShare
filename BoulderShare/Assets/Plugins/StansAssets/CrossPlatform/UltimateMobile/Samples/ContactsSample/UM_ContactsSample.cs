using SA.Android.Utilities;
using SA.CrossPlatform.App;
using UnityEngine;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
    public class UM_ContactsSample : MonoBehaviour
    {
        [SerializeField] Button m_LoadAllContactsAsyncButton = null;
        [SerializeField] Button m_LoadContactsAsyncButton = null;
        [SerializeField] Button m_LoadAllContactsButton = null;
        [SerializeField] Button m_LoadContactsButton = null;
        [SerializeField] Button m_LoadContactsCountButton = null;
        private UM_iContactsService m_Client;
        
        private void Start() 
        {
            m_Client = UM_Application.ContactsService;
            m_LoadAllContactsAsyncButton.onClick.AddListener(() => 
            {
                LoadAllContactsAsync();
            });
            m_LoadContactsAsyncButton.onClick.AddListener(() =>
            {
                LoadContactAsync(0, 5);
            });
            m_LoadAllContactsButton.onClick.AddListener(() =>
            {
                LoadAllContacts();
            });
            m_LoadContactsButton.onClick.AddListener(() =>
            {
                LoadContact(0, 5);
            });
            m_LoadContactsCountButton.onClick.AddListener(() =>
            {
                GetContactsCount();
            });
        }

        private void LoadAllContactsAsync() 
        {
            m_Client.Retrieve(result => 
            {
                LogContacts(result);
            });
        }

        private void LoadContactAsync(int index, int count)
        {
            m_Client.RetrieveContacts(index, count, result =>
            {
                LogContacts(result);
            });
        }

        private void LoadAllContacts()
        {
            SA.Android.Contacts.AN_ContactsResult result = SA.Android.Contacts.AN_ContactsContract.RetrieveAll();
            LogContacts(result);
        }

        private void LoadContact(int index, int count)
        {
            SA.Android.Contacts.AN_ContactsResult result = SA.Android.Contacts.AN_ContactsContract.Retrieve(index, count);
            LogContacts(result);
        }

        private void GetContactsCount()
        {   
           AN_Logger.Log("---------->");
           AN_Logger.Log("Contacts count " + m_Client.GetContactsCount().ToString());
        }

        private void LogContacts(SA.Android.Contacts.AN_ContactsResult result)
        {
            if (result.IsSucceeded)
            {
                foreach (var contact in result.Contacts)
                {
                    AN_Logger.Log("---------->");
                    AN_Logger.Log("contact.Name:" + contact.Name);
                    AN_Logger.Log("contact.Phone:" + contact.Phone);
                    AN_Logger.Log("contact.Email:" + contact.Email);
                }
            }
            else
            {
                AN_Logger.Log("Failed to load contacts: " + result.Error.FullMessage);
            }
        }

        private void LogContacts(UM_ContactsResult result)
        {
            if (result.IsSucceeded)
            {
                foreach (var contact in result.Contacts)
                {
                    AN_Logger.Log("---------->");
                    AN_Logger.Log("contact.Name:" + contact.Name);
                    AN_Logger.Log("contact.Phone:" + contact.Phone);
                    AN_Logger.Log("contact.Email:" + contact.Email);
                }
            }
            else
            {
                AN_Logger.Log("Failed to load contacts: " + result.Error.FullMessage);
            }
        }
    }
}