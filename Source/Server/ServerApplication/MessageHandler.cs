using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    using System;
    using System.Net.Sockets;
    using System.Text.Json;

    public static class MessageHandler
    {
        /// <summary>
        /// Handles the client message
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="message"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static string HandleClientMessage(Socket clientSocket, string message, ClientHandler handler = null)
        {
            Console.WriteLine("Message received: " + message);
            if (string.IsNullOrEmpty(message))
                return "Invalid command.";

            ConverterContainer container = JsonSerializer.Deserialize<ConverterContainer>(message);
            Console.WriteLine("Container: " + container.Type);
            Console.WriteLine("Container: " + container.JSON);
            //Console.WriteLine("JSON Convertion worked!");
            switch (container.Type.ToLower())
            {
                case "exit":
                    return new UserHandler().Exit(clientSocket, handler);
                case "edit":
                    return new UserHandler().Edit(clientSocket, container.JSON, handler);
                case "login":
                    return new UserHandler().Login(clientSocket, container.JSON, handler);
                case "register":
                    return new UserHandler().Register(clientSocket, container.JSON, handler);
                case "message":
                    return new UserHandler().SendMessage(clientSocket, container.JSON,handler);
                case "init_key_exchange":
                    return new UserHandler().InitKeyExchange(clientSocket, container.JSON, handler);
                case "request_message_history":
                    return new UserHandler().RequestMessageHistory(clientSocket, container.JSON, handler);
                case "request_groups":
                    return new UserHandler().RequestGroups(clientSocket, container.JSON, handler);
                case "request_users":
                    return new UserHandler().RequestUsers(clientSocket, container.JSON, handler);
                case "request_online_users":
                    return new UserHandler().RequestOnlineUsers(clientSocket, container.JSON, handler);
                case "create_chat":
                    return new UserHandler().CreateChat(clientSocket, container.JSON, handler);
                case "delete_chat":
                    return new UserHandler().DeleteChat(clientSocket, container.JSON, handler);
                case "demote_user":
                    return new UserHandler().DemoteUser(clientSocket, container.JSON, handler);
                case "promote_user":
                    return new UserHandler().PromoteUser(clientSocket, container.JSON, handler);
                case "ban_user":
                    return new UserHandler().BanUser(clientSocket, container.JSON, handler);
                case "unban_user":
                    return new UserHandler().UnbanUser(clientSocket, container.JSON, handler);
                case "kick_user":
                    return new UserHandler().KickUser(clientSocket, container.JSON, handler);
                case "reset_password":
                    return new UserHandler().ResetPassword(clientSocket, container.JSON, handler);
                case "is_admin":
                    return new UserHandler().IsAdmin(clientSocket, container.JSON, handler);
                default:
                    return JsonSerializer.Serialize(new ConverterContainer("invalid_command",""));
            }
        }
    }
}
