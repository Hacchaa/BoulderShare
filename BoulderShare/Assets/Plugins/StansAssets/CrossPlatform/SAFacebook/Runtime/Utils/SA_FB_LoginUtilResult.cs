using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Facebook
{
    public class SA_FB_LoginUtilResult : IGraphResult
    {
        readonly string m_error = string.Empty;
        readonly bool m_isSucceeded = false;

        public SA_FB_LoginUtilResult(bool isSucceeded)
        {
            m_isSucceeded = isSucceeded;
            if (!m_isSucceeded) m_error = "Operation is requires active session, make sure user is logged in";
        }

        public bool IsSucceeded => m_isSucceeded;

        public string Error => m_error;

        public IDictionary<string, object> ResultDictionary => new Dictionary<string, object>();

        public string RawResult => string.Empty;

        public bool Cancelled => false;

        public IList<object> ResultList => new List<object>();

        public Texture2D Texture => null;
    }
}
