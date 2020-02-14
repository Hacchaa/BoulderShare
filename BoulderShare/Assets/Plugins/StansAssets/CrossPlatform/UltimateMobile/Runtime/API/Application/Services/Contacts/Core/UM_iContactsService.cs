using System;

namespace SA.CrossPlatform.App
{
    /// <summary>
    /// Service object for using contacts API.
    /// </summary>
    public interface UM_iContactsService 
    {
        /// <summary>
        /// Retrieve's all contacts from a device contacts book.
        /// </summary>
        /// <returns>The operation result callback.</returns>
        /// <param name="callback">Callback.</param>
        void Retrieve(Action<UM_ContactsResult> callback);

        /// <summary>
        /// Retrieve's contacts from a device contacts book.
        /// </summary>
        /// <returns>The operation result callback.</returns>
        /// <param name="callback">Callback.</param>
        void RetrieveContacts(int index, int count, Action<UM_ContactsResult> callback);

        /// <summary>
        /// Get contacts count from a device contacts book.
        /// </summary>
        /// <returns>Count of contacts.</returns>
        int GetContactsCount();
    }
}