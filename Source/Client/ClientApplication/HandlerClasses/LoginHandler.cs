using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApplication
{
    /// <summary>
    /// Handles Login functunality. 
    /// </summary>
    public class LoginHandler
    {
        /// <summary>
        /// Attempts to log in on the server.
        /// </summary>
        /// <param name="ip">IP address of the server.</param>
        /// <param name="user">Username of the client.</param>
        /// <param name="email">Email of the client.</param>
        /// <param name="pw">Password of the client.</param>
        /// <returns>A new instance of ApplicationInfo if the login succeeds, null if not.</returns>
        public static async Task<ApplicationInfo> AttempLogin(string ip, string user, string email, string pw, MainWindow mainWindow) 
        {
            ApplicationInfo appInfo = new ApplicationInfo();

            appInfo.Client = await Client.CreateClient(ip);

            Console.WriteLine("Client created!");

            UserData ud = new UserData(user, email, pw);
            string userDataString = JsonSerializer.Serialize(ud);

            ConverterContainer cv = new ConverterContainer("login", userDataString);
            string finalString = JsonSerializer.Serialize(cv);



            // Send the ip, user, and pw to the server
            await appInfo.Client.SendMessage(finalString);

            // Receive the response from the server
            string response = await appInfo.Client.ReceiveMessage();

            ConverterContainer converter = JsonSerializer.Deserialize<ConverterContainer>(response);
            //Check the server response.
            switch (converter.Type)
            {
                case "login_failed":
                    appInfo.Client.DisconnectToServer();
                    return null;

                case "login_success":
                    ConverterContainer cc = JsonSerializer.Deserialize<ConverterContainer>(converter.JSON);
                    Dictionary<int, string> dict = JsonSerializer.Deserialize<Dictionary<int, string>>(cc.JSON);
                    appInfo.ID = int.Parse(cc.Type);
                    //Set the online users in ApplicationInfo.
                    foreach (KeyValuePair<int, string> kv in dict)
                    {
                        Console.WriteLine(kv.ToString());
                    }
                    appInfo.OnlineUsers = dict;

                    //Start thread to handle communication from now on.
                    appInfo.ListenThread = new Thread(()=>MessageHandler.run(appInfo, mainWindow));
                    appInfo.ListenThread.Start();


                    //Request all users registered on the server.
                    await appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_users", "")));
                    
                    //Check if client is admin or not.
                    await appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("is_admin", appInfo.ID.ToString())));

                    return appInfo;

                default:
                    return null;
            }
            
        }
    }
}
