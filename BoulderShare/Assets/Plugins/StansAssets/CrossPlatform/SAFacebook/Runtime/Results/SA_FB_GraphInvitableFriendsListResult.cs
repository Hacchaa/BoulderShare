using System.Collections;
using System.Collections.Generic;
using System;
using SA.Foundation.Templates;

namespace SA.Facebook
{
    public class SA_FB_GraphInvitableFriendsListResult : SA_FB_GraphResult
    {
        readonly List<SA_FB_User> m_users = new List<SA_FB_User>();

        public SA_FB_GraphInvitableFriendsListResult(IGraphResult graphResult)
            : base(graphResult)
        {
            if (m_error == null)
                try
                {
                    var RAW = Json.Deserialize(RawResult) as IDictionary;
                    var friends = RAW[FriendsListKey] as IDictionary;
                    var flist = friends["data"] as IList;

                    for (var i = 0; i < flist.Count; i++)
                    {
                        var user = new SA_FB_User(flist[i] as IDictionary);
                        m_users.Add(user);
                    }

                    ParseResultId(RAW);
                    ParsePaginatedResult(friends);
                }
                catch (Exception ex)
                {
                    m_error = new SA_Error(5, "Failed to parse friends data " + ex.Message);
                }
        }

        /// <summary>
        /// List of loaded user model's
        /// </summary>
        public List<SA_FB_User> Users => m_users;

        protected virtual string FriendsListKey => "invitable_friends";
    }
}
