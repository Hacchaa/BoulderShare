using System.Collections;
using System;
using SA.Foundation.Templates;

namespace SA.Facebook
{
    public class SA_FB_GraphFriendsListResult : SA_FB_GraphInvitableFriendsListResult
    {
        readonly int m_TotalFriendsCount = 0;

        public SA_FB_GraphFriendsListResult(IGraphResult graphResult)
            : base(graphResult)
        {
            if (m_error == null)
                try
                {
                    var JSON = Json.Deserialize(RawResult) as IDictionary;
                    var body = JSON[FriendsListKey] as IDictionary;

                    if (body.Contains("summary"))
                    {
                        var summary = body["summary"] as IDictionary;
                        if (summary.Contains("total_count")) m_TotalFriendsCount = Convert.ToInt32(summary["total_count"]);
                    }
                }
                catch (Exception ex)
                {
                    m_error = new SA_Error(5, "Failed to parse friends data " + ex.Message);
                }
        }

        public int TotalFriendsCount => m_TotalFriendsCount;

        protected override string FriendsListKey => "friends";
    }
}
