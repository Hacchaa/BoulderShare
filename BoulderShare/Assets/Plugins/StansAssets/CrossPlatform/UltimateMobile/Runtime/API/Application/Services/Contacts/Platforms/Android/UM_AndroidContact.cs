using System;
using SA.Android.Contacts;

namespace SA.CrossPlatform.App
{
    [Serializable]
    internal class UM_AndroidContact : UM_AbstractContact, UM_iContact
    {
        public UM_AndroidContact(AN_ContactInfo contact) {
            m_name = contact.Name;
            m_email = contact.Email;
            m_phone = contact.Phone;
        }
    }
}