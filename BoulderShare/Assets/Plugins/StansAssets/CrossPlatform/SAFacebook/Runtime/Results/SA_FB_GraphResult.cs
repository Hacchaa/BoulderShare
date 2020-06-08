using System;
using System.Collections;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.Facebook
{
    /// <summary>
    /// Abstract Gprah API result class
    /// </summary>
    public abstract class SA_FB_GraphResult : SA_FB_Result
    {
        string m_previous;
        string m_next;

        string m_before;
        string m_after;

        string m_id;

        public SA_FB_GraphResult(IGraphResult graphResult)
            : base(graphResult) { }

        protected void ParsePaginatedResult(IDictionary paginatedResult)
        {
            var paging = paginatedResult["paging"] as IDictionary;
            var cursors = paging["cursors"] as IDictionary;

            if (paging.Contains("previous")) m_previous = Convert.ToString(paging["previous"]);

            if (paging.Contains("next")) m_next = Convert.ToString(paging["next"]);

            m_before = Convert.ToString(cursors["before"]);
            m_after = Convert.ToString(cursors["after"]);
        }

        protected void ParseResultId(IDictionary rawDict)
        {
            m_id = Convert.ToString(rawDict["id"]);
        }

        /// <summary>
        /// Request Id
        /// </summary>
        public string Id => m_id;

        /// <summary>
        /// Full request URL for a next page
        /// </summary>
        public string Next => m_next;

        /// <summary>
        /// True if request has next page of results
        /// </summary>
        public bool HasNext => !string.IsNullOrEmpty(m_next);

        /// <summary>
        /// Full request URL for a previous page
        /// </summary>
        public string Previous => m_previous;

        /// <summary>
        /// True if request has previous page of results
        /// </summary>
        public bool HasPrevious => !string.IsNullOrEmpty(m_previous);

        /// <summary>
        /// Request before page pointer
        /// </summary>
        public string Before => m_before;

        /// <summary>
        /// Request after page pointer
        /// </summary>
        public string After => m_after;

        /// <summary>
        /// Generated before cursor pointer
        /// </summary>
        public SA_FB_Cursor BeforeCursorPointer => new SA_FB_Cursor(SA_FB_CursorType.before, m_before);

        /// <summary>
        /// Generated after cursor pointer
        /// </summary>
        public SA_FB_Cursor AfterCursorPointer => new SA_FB_Cursor(SA_FB_CursorType.after, m_after);
    }
}
