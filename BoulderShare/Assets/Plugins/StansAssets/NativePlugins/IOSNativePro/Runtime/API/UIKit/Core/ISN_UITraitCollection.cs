using SA.iOS.UIKit.Internal;
using SA.iOS.Utilities;

namespace SA.iOS.UIKit
{
    /// <summary>
    /// The iOS interface environment for your app,
    /// defined by traits such as horizontal and vertical size class, display scale, and user interface idiom.
    /// </summary>
    public class ISN_UITraitCollection : ISN_NativeObject
    {
        internal ISN_UITraitCollection(ulong hash) : base(hash)
        {
        }

        /// <summary>
        /// The style associated with the user interface.
        /// </summary>
        public ISN_UIUserInterfaceStyle UserInterfaceStyle
        {
            get { return ISN_UILib.API.TraitCollection_UserInterfaceStyle(NativeHashCode); }
        }
        
    }
}