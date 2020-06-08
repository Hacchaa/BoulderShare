using UnityEngine;
using UnityEngine.UI;
using SA.CrossPlatform.GameServices;

namespace SA.CrossPlatform.Samples
{
    public class UM_GameServiceAchievmentsExample : MonoBehaviour
    {
        [SerializeField]
        Button m_NativeUIButton = null;
        [SerializeField]
        Button m_LoadButton = null;
        [SerializeField]
        UM_AchievmentsMetaView m_AchievmentMetaView = null;

        void Start()
        {
            m_AchievmentMetaView.gameObject.SetActive(false);
            m_LoadButton.onClick.AddListener(LoadMeta);
            m_NativeUIButton.onClick.AddListener(() =>
            {
                var client = UM_GameService.AchievementsClient;
                client.ShowUI(result =>
                {
                    if (result.IsSucceeded)
                    {
                        Debug.Log("Operation completed successfully!");
                    }
                    else
                    {
                        Debug.LogError($"Failed to show achievements UI {result.Error.FullMessage}");
                    }
                });
            });
        }

        void LoadMeta()
        {
            var client = UM_GameService.AchievementsClient;
            client.Load(result =>
            {
                if (result.IsSucceeded)
                {
                    foreach (var achievement in result.Achievements)
                    {
                        PrintAchievementInfo(achievement);
                        var view = Instantiate(m_AchievmentMetaView.gameObject, m_AchievmentMetaView.transform.parent);
                        view.SetActive(true);
                        view.transform.localScale = Vector3.one;
                        var meta = view.GetComponent<UM_AchievmentsMetaView>();
                        meta.SetTitle(achievement.Name + " / " + achievement.State);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load achievements meta {result.Error.FullMessage}");
                }
            });
        }

        void PrintAchievementInfo(UM_iAchievement achievement)
        {
            Debug.Log("------------------------------------------------");
            Debug.Log($"achievement.Identifier: {achievement.Identifier}");
            Debug.Log($"achievement.Name: {achievement.Name}");
            Debug.Log($"achievement.State: {achievement.State}");
            Debug.Log($"achievement.Type: {achievement.Type}");
            Debug.Log($"achievement.TotalSteps: {achievement.TotalSteps}");
            Debug.Log($"achievement.CurrentSteps: {achievement.CurrentSteps}");
        }
    }
}
