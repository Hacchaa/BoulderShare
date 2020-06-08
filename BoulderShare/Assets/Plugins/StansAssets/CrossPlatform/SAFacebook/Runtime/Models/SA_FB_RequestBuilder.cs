using System.Text;

namespace SA.Facebook
{
    /// <summary>
    /// Class used to simplify Facebook Graph API request building
    /// </summary>
    public class SA_FB_RequestBuilder
    {
        readonly StringBuilder m_Request;

        public SA_FB_RequestBuilder(string request)
        {
            m_Request = new StringBuilder(request);
        }

        /// <summary>
        /// Adds facebook Graph API command to a current request
        /// </summary>
        public void AddCommand(string command, params object[] args)
        {
            m_Request.Append(".");
            m_Request.Append(command);
            m_Request.Append("(");

            for (var i = 0; i < args.Length; i++)
            {
                m_Request.Append(args[i]);
                if (i != args.Length - 1) m_Request.Append(",");
            }

            m_Request.Append(")");
        }

        /// <summary>
        /// Adds a limit command
        /// </summary>
        public void AddLimit(int limit)
        {
            AddCommand("limit", limit);
        }

        /// <summary>
        /// Add pagination cursor
        /// </summary>
        public void AddCursor(SA_FB_Cursor cursor)
        {
            if (cursor != null) AddCommand(cursor.Type.ToString(), cursor.Value);
        }

        /// <summary>
        /// Returns built request string
        /// </summary>
        public string RequestString => m_Request.ToString();
    }
}
