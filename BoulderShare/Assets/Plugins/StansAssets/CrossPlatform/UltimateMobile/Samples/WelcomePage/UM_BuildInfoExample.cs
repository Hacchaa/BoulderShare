using SA.CrossPlatform.App;
using SA.iOS.Foundation;
using SA.iOS.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class UM_BuildInfoExample : MonoBehaviour
{
    [SerializeField] private Text m_LocaleInfoLabel = null;
    [SerializeField] private Text m_BuildInfoLabel = null;

    private void Start()
    {
        PrintBuildInfo();
        PrintLocaleInfo();
        PrintTimezoneInfo();
        
        UM_iBuildInfo buildInfo = UM_Build.Info;

        Debug.Log("buildInfo.Identifier: " + buildInfo.Identifier);
        Debug.Log("buildInfo.Version: " + buildInfo.Version);
    }

    private void PrintBuildInfo()
    {
        var buildInfo = UM_Build.Info;
        m_BuildInfoLabel.text = string.Format("<b>Build Info:</b> " +
                                              "\n Identifier: {0} " +
                                              "\n Version: {1}",
            buildInfo.Identifier,
            buildInfo.Version);
    }

    private void PrintLocaleInfo()
    {
        var currentLocale = UM_Locale.GetCurrentLocale();
        m_LocaleInfoLabel.text = string.Format("<b>Locale Info:</b> " +
                                           "\n CountryCode: {0} " +
                                           "\n LanguageCode: {1}" +
                                           "\n CurrencyCode: {2}" +
                                           "\n CurrencySymbol: {3}",
            currentLocale.CountryCode,
            currentLocale.LanguageCode,
            currentLocale.CurrencyCode,
            currentLocale.CurrencySymbol);
    }

    private void PrintTimezoneInfo()
    {
        var zone = ISN_NSTimeZone.LocalTimeZone;
        ISN_Logger.Log("LocalTimeZone.Name: " + zone.Name);
        ISN_Logger.Log("LocalTimeZone.Description: " + zone.Description);
        ISN_Logger.Log("LocalTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);


        zone = ISN_NSTimeZone.DefaultTimeZone;
        ISN_Logger.Log("DefaultTimeZone.Name: " + zone.Name);
        ISN_Logger.Log("DefaultTimeZone.Description: " + zone.Description);
        ISN_Logger.Log("DefaultTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);


        zone = ISN_NSTimeZone.SystemTimeZone;
        ISN_Logger.Log("SystemTimeZone.Name: " + zone.Name);
        ISN_Logger.Log("SystemTimeZone.Description: " + zone.Description);
        ISN_Logger.Log("SystemTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);
    }
}
