namespace SA.CrossPlatform.Advertisement
{
    public static class UM_GoogleAdsClientProxy
    {
        private static UM_iAdsClient s_AdsClient;

        public static void RegisterAdsClient(UM_iAdsClient client)
        {
            s_AdsClient = client;
        }

        internal static UM_iAdsClient AdsClient
        {
            get { return s_AdsClient; }
        }
    }
}