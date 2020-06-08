namespace SA.CrossPlatform.Analytics
{
    /// <summary>
    /// Main entry point for the Advertisement Services APIs.
    /// </summary>
    public static class UM_AnalyticsService
    {
        static UM_IAnalyticsClient s_Client;

        /// <summary>
        /// Analytics client.
        /// </summary>
        public static UM_IAnalyticsClient Client
        {
            get
            {
                if (s_Client == null)
                {
                    s_Client = new UM_MasterAnalyticsClient();
                    UM_AnalyticsInternal.Init();
                }

                return s_Client;
            }
        }
    }
}
