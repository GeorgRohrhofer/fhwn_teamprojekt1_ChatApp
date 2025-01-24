using System.Text.Json;

namespace ClientApplication
{
    /// <summary>
    /// Class used for communication between client and server.
    /// </summary>
    public class ConverterContainer
    {
        /// <summary>
        /// Constructor for a new instance of ConverterContainer.
        /// </summary>
        /// <param name="type">Type of request/response.</param>
        /// <param name="json">Data string attatched to message.</param>
        public ConverterContainer(string type, string json)
        {
            this.Type = type;
            this.JSON = json;
        }

        /// <summary>
        /// Property that holds request/response type.
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Property that holds data attachment.
        /// </summary>
        public string JSON
        {
            get;
            set;
        }
    }
}