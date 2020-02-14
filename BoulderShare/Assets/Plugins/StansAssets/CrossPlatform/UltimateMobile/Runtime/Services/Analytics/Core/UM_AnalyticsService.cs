namespace SA.CrossPlatform.Analytics
{
    /// <summary>
    /// Main entry point for the Advertisement Services APIs. 
    /// </summary>
    public static class UM_AnalyticsService
    {
        private static UM_iAnalyticsClient s_Client;

        /// <summary>
        /// Returns analytics client.
        /// </summary>
        public static UM_iAnalyticsClient Client {
            get {
                if(s_Client == null) 
                {
                    s_Client = new UM_MasterAnalyticsClient();
                    UM_AnalyticsInternal.Init();
                }
                return s_Client;
            }
        }

    }
}