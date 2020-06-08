using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.Facebook
{
    public class SA_FB_LoginResult : SA_FB_Result
    {
        readonly string m_userId;
        readonly SA_AccessToken m_accessToken;

        public SA_FB_LoginResult(ILoginResult result)
            : base(result)
        {
            if (m_error == null)
            {
                m_userId = result.AccessToken.UserId;
                m_accessToken = result.AccessToken;
            }
        }

        public string UserId => m_userId;

        public SA_AccessToken AccessToken => m_accessToken;
    }
}
