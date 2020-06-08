namespace SA.Facebook
{
    /// <summary>
    /// The representation of Graph API paginated cursor
    /// </summary>
    public class SA_FB_Cursor
    {
        /// <summary>
        /// Cursor type
        /// </summary>
        public SA_FB_CursorType Type { get; }

        /// <summary>
        /// Cursor value
        /// </summary>
        public string Value { get; }

        public SA_FB_Cursor(SA_FB_CursorType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
