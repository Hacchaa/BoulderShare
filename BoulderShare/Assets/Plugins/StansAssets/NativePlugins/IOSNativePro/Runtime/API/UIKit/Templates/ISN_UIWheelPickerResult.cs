using System;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.iOS.UIKit
{
    /// <summary>
    /// This type for saving data from ISN_UIWheelPicker callback.
    /// </summary>
    [Serializable]
    public class ISN_UIWheelPickerResult : SA_Result
    {
        [SerializeField]
        protected string m_Value;
        [SerializeField]
        protected string m_State;

        /// <summary>
        /// Get chosen value from ISN_UIWheelPicker callback.
        /// </summary>
        public string Value => m_Value;

        /// <summary>
        /// Get current state of ISN_UIWheelPicker callback.
        /// </summary>
        public ISN_UIWheelPickerStates State
        {
            get
            {
                if (!string.IsNullOrEmpty(m_State) && m_State.Equals(ISN_UIWheelPickerStates.Done.ToString())) return ISN_UIWheelPickerStates.Done;

                if (!string.IsNullOrEmpty(m_State) && m_State.Equals(ISN_UIWheelPickerStates.InProgress.ToString())) return ISN_UIWheelPickerStates.InProgress;

                return ISN_UIWheelPickerStates.Canceled;
            }
        }
    }
}
