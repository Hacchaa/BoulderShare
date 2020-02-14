namespace SA.iOS.AuthenticationServices
{
    public enum ISN_ASAuthorizationError
    {
        /// <summary>
        /// The authorization attempt failed for an unknown reason.
        /// </summary>
        Unknown = 1000,
        
        /// <summary>
        /// The user canceled the authorization attempt.
        /// </summary>
        Canceled = 1001,
        
        /// <summary>
        /// The authorization request received an invalid response.
        /// </summary>
        Response = 1002,
        
        /// <summary>
        /// The authorization request wasn’t handled.
        /// </summary>
        NotHandled = 1003,
        
        /// <summary>
        /// The authorization attempt failed.
        /// </summary>
        Failed = 1004,
    }
}
