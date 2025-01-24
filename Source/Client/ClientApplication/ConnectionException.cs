
namespace ClientApplication
{
    /// <summary>
    /// Custome class for Exception during connection to server. 
    /// </summary>
    [Serializable]
    internal class ConnectionException : Exception
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string? message) : base(message)
        {
        }

        public ConnectionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}