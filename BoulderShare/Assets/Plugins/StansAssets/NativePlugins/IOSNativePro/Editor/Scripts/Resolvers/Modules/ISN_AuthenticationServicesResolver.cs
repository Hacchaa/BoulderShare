using SA.iOS.XCode;

namespace SA.iOS
{
    public class ISN_AuthenticationServicesResolver : ISN_APIResolver
    {
        protected override ISN_XcodeRequirements GenerateRequirements() 
        {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.AuthenticationServices));
            return requirements;
        }

        protected override string LibFolder 
        {
            get 
            { 
                return "AuthenticationServices/"; 
            } 
        }

        public override bool IsSettingsEnabled {
            get { return ISD_API.Capability.SignInWithApple.Enabled; }
            set {
                ISD_API.Capability.SignInWithApple.Enabled = value;
                ISD_Settings.Save();
            }
        }
        
        public override string DefineName 
        { 
            get 
            { 
                return "AUTHENTICATION_SERVICES_API_ENABLED"; 
            } 
        }
    }
}