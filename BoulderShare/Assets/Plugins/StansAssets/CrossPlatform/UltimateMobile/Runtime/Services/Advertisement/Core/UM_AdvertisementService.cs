using UnityEngine;
using System.Collections.Generic;

namespace SA.CrossPlatform.Advertisement
{
    /// <summary>
    /// Main entry point for the Advertisement Services APIs.
    /// </summary>
    public class UM_AdvertisementService
    {
        static readonly Dictionary<UM_AdPlatform, UM_iAdsClient> s_CreatedClients = new Dictionary<UM_AdPlatform, UM_iAdsClient>();

        /// <summary>
        /// Returns advertisement client based on platform.
        /// </summary>
        /// <param name="platform">Advertisement platform.</param>
        /// <returns>Created advertisement client for the specified platform</returns>
        public static UM_iAdsClient GetClient(UM_AdPlatform platform)
        {
            if (s_CreatedClients.ContainsKey(platform))
                return s_CreatedClients[platform];

            var client = CreateClient(platform);
            s_CreatedClients.Add(platform, client);

            return client;
        }

        static UM_iAdsClient CreateClient(UM_AdPlatform platform)
        {
            if (Application.isEditor)
                return new UM_EditorAdsClient();

            switch (platform)
            {
                case UM_AdPlatform.Google:
                    return UM_AdsClientProxy.GoogleAdsClient;
                case UM_AdPlatform.Unity:
                    return UM_AdsClientProxy.UnityAdsClient;
            }

            return null;
        }
    }
}
