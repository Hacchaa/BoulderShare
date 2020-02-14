using SA.Android.Vending.BillingClient;

namespace SA.CrossPlatform.InApp
{
    public class UM_AndroidInAppUtilities
    {
        public AN_BillingClient ActiveBillingClient
        {
            get
            {
                var client = UM_InAppService.Client;
                if (client is UM_AndroidInAppClient)
                {
                    return (client as UM_AndroidInAppClient).BillingClient;
                }

                return null;
            }
        }
    }
}