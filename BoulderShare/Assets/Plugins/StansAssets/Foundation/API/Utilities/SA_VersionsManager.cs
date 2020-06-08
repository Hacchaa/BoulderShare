////////////////////////////////////////////////////////////////////////////////
//  
// @module Stan's Assets Commons Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using System;
using SA.Foundation.Utility;

namespace SA.Foundation.Editor
{
    public static class SA_VersionsManager
    {
        public static int ParceMagorVersion(string stringVersionId)
        {
            var versions = stringVersionId.Split(new char[] { '.', '/' });
            var intVersion = int.Parse(versions[0]) * 100;
            return intVersion;
        }

        static int GetMagorVersionCode(string versionFilePath)
        {
            var stringVersionId = SA_FilesUtil.Read(versionFilePath);
            return ParceMagorVersion(stringVersionId);
        }

        public static int ParceVersion(string stringVersionId)
        {
            var versions = stringVersionId.Split(new char[] { '.', '/' });
            var intVersion = int.Parse(versions[0]) * 100 + int.Parse(versions[1]) * 10;
            return intVersion;
        }

        static int GetVersionCode(string versionFilePath)
        {
            var stringVersionId = SA_FilesUtil.Read(versionFilePath);
            return ParceVersion(stringVersionId);
        }

        static string GetStringVersionId(string versionFilePath)
        {
            if (SA_FilesUtil.IsFileExists(versionFilePath))
                return SA_FilesUtil.Read(versionFilePath);
            else
                return "0.0";
        }
    }
}

#endif
