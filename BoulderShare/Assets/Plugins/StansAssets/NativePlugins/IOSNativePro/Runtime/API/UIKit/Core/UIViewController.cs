using System;

namespace SA.iOS.UIKit
{
    /// <summary>
    /// An object that manages a view hierarchy for your UIKit app.
    /// </summary>
    [Serializable]
    public class UIViewController
    {
        /// <summary>
        /// Presents a view controller modally.
        ///
        /// In a horizontally regular environment, the view controller is presented in the style specified
        /// by the <see cref="ModalPresentationStyle"/> property. In a horizontally compact environment,
        /// the view controller is presented full screen by default.
        /// If you associate an adaptive delegate with the presentation controller associated with the object
        /// in viewControllerToPresent, you can modify the presentation style dynamically.
        ///
        /// The object on which you call this method may not always be the one that handles the presentation.
        /// Each presentation style has different rules governing its behavior.
        /// For example, a full-screen presentation must be made by a view controller that itself covers the entire screen.
        /// If the current view controller is unable to fulfill a request,
        /// it forwards the request up the view controller hierarchy to its nearest parent,
        /// which can then handle or forward the request.
        ///
        /// Before displaying the view controller, this method resizes the presented view controller's
        /// view based on the presentation style. For most presentation styles,
        /// the resulting view is then animated onscreen using the transition style in the
        /// modalTransitionStyle property of the presented view controller.
        /// For custom presentations, the view is animated onscreen using the presented view controller’s
        /// transitioning delegate. For current context presentations,
        /// the view may be animated onscreen using the current view controller’s transition style.
        ///
        ///  The completion handler is called after the viewDidAppear: method is called on the presented view controller.
        /// </summary>
        /// <param name="viewControllerToPresent"></param>
        /// <param name="animated"></param>
        /// <param name="completion"></param>
        public void PresentViewController(UIViewController viewControllerToPresent, bool animated, Action completion)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The presentation style for modally presented view controllers.
        ///
        /// The presentation style determines how a modally presented view controller is displayed onscreen.
        /// In a horizontally compact environment, modal view controllers are always presented full-screen.
        /// In a horizontally regular environment, there are several different presentation options.
        ///
        /// The default value for this property is <see cref="ISN_UIModalPresentationStyle.Automatic"/>
        /// For a list of possible presentation styles, and their compatibility with the available transition styles,
        /// see the <see cref="ISN_UIModalPresentationStyle"/> enum descriptions.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public ISN_UIModalPresentationStyle ModalPresentationStyle
        {
            get { throw new NotImplementedException(); }
        }
    }
}