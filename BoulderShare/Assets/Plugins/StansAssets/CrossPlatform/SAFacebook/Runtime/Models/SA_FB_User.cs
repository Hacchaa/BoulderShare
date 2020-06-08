////////////////////////////////////////////////////////////////////////////////
//
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets)
// @support stans.assets@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Network.Web;
using UnityEngine.Assertions;

namespace SA.Facebook
{
    /// <summary>
    /// Facebook user model
    /// Contains parsed user fields from a Facebook API response,
    /// and additional methods to retrieve mode data based on current model state
    /// like for example generate user avatar url, etc
    /// </summary>
    public class SA_FB_User
    {
        readonly Dictionary<SA_FB_ProfileImageSize, string> m_PicturesUrls = new Dictionary<SA_FB_ProfileImageSize, string>();

        //--------------------------------------
        // INITIALIZE
        //--------------------------------------

        public SA_FB_User(IDictionary json)
        {
            ParseData(json);
        }

        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------

        /// <summary>
        /// Generates user profile image URL
        /// </summary>
        /// <param name="size">Requested profile image size.</param>
        /// <param name="callback">Request callback.</param>
        public void GetProfileUrl(SA_FB_ProfileImageSize size, Action<string> callback)
        {
            if (m_PicturesUrls.ContainsKey(size))
            {
                callback.Invoke(m_PicturesUrls[size]);
                return;
            }

            SA_FB.GraphAPI.ResolveProfileImageUrl(Id, size, (url) =>
            {
                m_PicturesUrls.Add(size, url);
                callback.Invoke(url);
            });
        }

        /// <summary>
        /// Download user profile image of a given size
        /// </summary>
        /// <param name="size">Requested profile image size</param>
        /// <param name="callback">Callback with user Texture2D profile image</param>
        public void GetProfileImage(SA_FB_ProfileImageSize size, Action<Texture2D> callback)
        {
            GetProfileUrl(size, url => { SA_CachedRequestsFactory.GetTexture2D(url, callback); });
        }

        //--------------------------------------
        //  GET/SET
        //--------------------------------------

        public string Id { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public SA_FB_Gender Gender { get; set; } = SA_FB_Gender.Male;
        public string AgeRange { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;

        //--------------------------------------
        //  PRIVATE METHODS
        //--------------------------------------

        void ParseData(IDictionary json)
        {
            if (json.Contains("id")) Id = Convert.ToString(json["id"]);

            if (json.Contains("birthday"))
            {
                var birthday = string.Empty;
                try
                {
                    birthday = Convert.ToString(json["birthday"]);
                    Birthday = DateTime.Parse(birthday);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Failed to Parse birthday:" + birthday + " with error:" + ex.Message);
                }
            }

            if (json.Contains("name")) Name = Convert.ToString(json["name"]);

            if (json.Contains("first_name")) FirstName = Convert.ToString(json["first_name"]);

            if (json.Contains("last_name")) LastName = Convert.ToString(json["last_name"]);

            if (json.Contains("username")) UserName = Convert.ToString(json["username"]);

            if (json.Contains("link")) ProfileUrl = Convert.ToString(json["link"]);

            if (json.Contains("email")) Email = Convert.ToString(json["email"]);

            if (json.Contains("locale")) Locale = Convert.ToString(json["locale"]);

            if (json.Contains("location"))
            {
                var loc = json["location"] as IDictionary;
                Location = Convert.ToString(loc["name"]);
            }

            if (json.Contains("gender"))
            {
                var g = Convert.ToString(json["gender"]);
                if (g.Equals("male"))
                    Gender = SA_FB_Gender.Male;
                else
                    Gender = SA_FB_Gender.Female;
            }

            if (json.Contains("age_range"))
            {
                var age = json["age_range"] as IDictionary;
                Assert.IsNull(age);
                AgeRange = age.Contains("min") ? age["min"].ToString() : "0";
                AgeRange += "-";
                AgeRange += age.Contains("max") ? age["max"].ToString() : "1000";
            }

            if (json.Contains("picture"))
                if (json["picture"] is IDictionary picDict && picDict.Contains("data"))
                    if (picDict["data"] is IDictionary data && data.Contains("url"))
                        PictureUrl = Convert.ToString(data["url"]);
        }
    }
}
