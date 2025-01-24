using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class that handles sending a new message to the server.
    /// </summary>
    public class SendMessageHandler
    {
        /// <summary>
        /// Attempts to send message.
        /// </summary>
        /// <param name="messageText">Text to send to the server.</param>
        /// <param name="groupID">Group to send the message to.</param>
        /// <param name="appInfo">ApplicationInfo instance that provides Client, UserID, etc.</param>
        public static async void AttemptSendMessage(string messageText, int groupID,  ApplicationInfo appInfo)
        {
            string text = messageText;
            ConverterContainer cc = new ConverterContainer(groupID.ToString(), text);
            cc = new ConverterContainer("message", JsonSerializer.Serialize(cc));
            await appInfo.Client.SendMessage(JsonSerializer.Serialize(cc));
            Console.WriteLine("Send data: " + JsonSerializer.Serialize(cc));
        }
    }
}
