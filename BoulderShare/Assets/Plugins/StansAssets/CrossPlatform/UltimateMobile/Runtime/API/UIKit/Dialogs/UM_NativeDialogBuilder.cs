using System;

namespace SA.CrossPlatform.UI 
{
    public class UM_NativeDialogBuilder
    {
        public class Button
        {
            public string Title;
            public Action ButtonAction;

            public Button(string title, Action buttonAction) {
                Title = title;
                ButtonAction = buttonAction;
            }
        }
        
        private string m_Title;
        private string m_Message;

        private Button m_NeutralButton;
        private Button m_PositiveButton;
        private Button m_NegativeButton;
        private Button m_DestructiveButton;
        
        /// <summary>
        /// Create new native dialog builder instance.
        /// </summary>
        /// <param name="title">Alert Title.</param>
        /// <param name="message">Alert Message</param>
        public UM_NativeDialogBuilder(string title, string message) 
        {
            m_Title = title;
            m_Message = message;
        }

        /// <summary>
        /// Set alert Title.
        /// </summary>
        /// <param name="title">New alert title.</param>
        public void SetTitle(string title) 
        {
            m_Title = title;
        }

        /// <summary>
        /// Set alert Message.
        /// </summary>
        /// <param name="message">New alert message.</param>
        public void SetMessage(string message)
        {
            m_Message = message;
        }
        
        /// <summary>
        /// Alert Title.
        /// </summary>
        public string Title 
        {
            get { return m_Title; }
        }

        /// <summary>
        /// Alert Message.
        /// </summary>
        public string Message 
        {
            get { return m_Message; }
        }

        /// <summary>
        /// Gets the neutral button.
        /// </summary>
        public Button NeutralButton 
        {
            get { return m_NeutralButton; }
        }

        /// <summary>
        /// Gets the positive button.
        /// </summary>
        public Button PositiveButton 
        {
            get { return m_PositiveButton; }
        }

        /// <summary>
        /// Gets the negative button.
        /// </summary>
        public Button NegativeButton 
        {
            get { return m_NegativeButton; }
        }

        /// <summary>
        /// Gets the destructive button.
        /// </summary>
        public Button DestructiveButton 
        {
            get { return m_DestructiveButton; }
        }

        /// <summary>
        /// Set button with default style.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listener</param>
        public void SetNeutralButton(string text, Action callback) 
        {
            m_NeutralButton = new Button(text, callback);
        }

        /// <summary>
        /// Set button with positive style.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listener</param>
        public void SetPositiveButton(string text, Action callback) 
        {
            m_PositiveButton = new Button(text, callback);
        }

        /// <summary>
        /// Set button with negative style, 
        /// that indicates the action cancels the operation and leaves things unchanged.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listener</param>
        public void SetNegativeButton(string text, Action callback) 
        {
            m_NegativeButton = new Button(text, callback);
        }

        /// <summary>
        /// Set button with destructive style, 
        /// that indicates the action might change or delete data.
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="callback">click listener</param>
        public void SetDestructiveButton(string text, Action callback)
        {
            m_DestructiveButton = new Button(text, callback);
        }

        /// <summary>
        /// Build the dialog based on a builder properties
        /// </summary>
        /// <returns></returns>
        public UM_iUIDialog Build() 
        {
            return UM_DialogsFactory.CreateDialog(this);
        }
    }
}