namespace SA.CrossPlatform.InApp
{
    public enum UM_TransactionState
    {
        /// <summary>
        /// Unknown Transaction state.
        /// </summary>
        Unspecified,
        
        /// <summary>
        /// Transaction finished shamefully. 
        /// </summary>
        Purchased,
        
        /// <summary>
        /// Transaction was resorted.
        /// </summary>
        Restored,
        
        /// <summary>
        /// Transaction Failed.
        /// </summary>
        Failed,
        
        /// <summary>
        /// (Android) Transaction is pending and the player will be informed when it's done.
        /// (iOS) A transaction that is in the queue, but its final status is pending external action such as Ask to Buy.
        /// </summary>
        Pending,
        
        [System.Obsolete("Deferred key is deprecated, use Pending instead")]
        Deferred
    }
}