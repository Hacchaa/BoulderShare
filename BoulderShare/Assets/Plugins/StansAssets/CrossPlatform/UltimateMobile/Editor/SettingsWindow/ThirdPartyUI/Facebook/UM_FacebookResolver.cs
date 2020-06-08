using SA.Facebook;
using SA.Foundation.Editor;

namespace SA.CrossPlatform.Editor
{
    class UM_FacebookResolver : SA_iAPIResolver
    {
        public bool IsSettingsEnabled
        {
            get => SA_FB.IsSDKInstalled;

            set { }
        }

        public void ResetRequirementsCache() { }
    }
}
