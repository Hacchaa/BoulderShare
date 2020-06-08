namespace SA.CrossPlatform.App
{
    /// <summary>
    /// Phone contact record model.
    /// </summary>
    public interface UM_iContact
    {
        /// <summary>
        /// Contact name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Phone number
        /// </summary>
        string Phone { get; }

        /// <summary>
        /// The email address
        /// </summary>
        string Email { get; }
    }
}
