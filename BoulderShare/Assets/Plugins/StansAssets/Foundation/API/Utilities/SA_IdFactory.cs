////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Linq;

namespace SA.Foundation.Utility
{
    public class SA_IdFactory
    {
        const string PP_ID_KEY = "SA.Common.Utility.SA_IdFactory_Key";

        /// <summary>
        /// Generates unique <see cref="PlayerPrefs"/> based incremental Id.
        /// <see cref="PlayerPrefs"/> is used to store previous id. 
        /// </summary>
        public static int NextId
        {
            get
            {
                var id = 1;
                if (PlayerPrefs.HasKey(PP_ID_KEY))
                {
                    id = PlayerPrefs.GetInt(PP_ID_KEY);
                    id++;
                }

                PlayerPrefs.SetInt(PP_ID_KEY, id);
                return id;
            }
        }

        /// <summary>
        /// Generates a random string
        /// </summary>
        public static string RandomString
        {
            get
            {
                var chars = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var result = string.Empty;
                for (var i = 0; i < 20; i++)
                {
                    var a = Random.Range(0, chars.Length);
                    result = result + chars[a];
                }

                return result;
            }
        }
    }
}
