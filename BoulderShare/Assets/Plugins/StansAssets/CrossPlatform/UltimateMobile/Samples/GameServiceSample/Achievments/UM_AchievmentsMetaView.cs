using UnityEngine;
using UnityEngine.UI;

namespace SA.CrossPlatform.Samples
{
    public class UM_AchievmentsMetaView : MonoBehaviour
    {
        [SerializeField]
        Text m_Title = null;

        public void SetTitle(string title)
        {
            m_Title.text = title;
        }
    }
}
