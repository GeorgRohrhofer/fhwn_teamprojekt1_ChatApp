using ClientApplication;
using ClientApplication.Pages;
using ClientApplication.user_controls;
using ServerApplication.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

/// <summary>
/// Class that provides the thread method to listen for server messages.
/// </summary>
public class MessageHandler
{

    Dictionary<int, string> openMessages;
    ChatWindowPage _page;
    public static ChatMessageList messageList;
    public static OnlineUsersView userOverview = new OnlineUsersView();



    /// <summary>
    /// Method for the listener thread.
    /// </summary>
    /// <param name="appInfo"> Class that holds information about the current application instance.</param>
    /// <exception cref="ConnectionException"> Exception when an error with the connection occures.</exception>
    public static async void run(ApplicationInfo appInfo, MainWindow mainWindow)
    {
        ConfigFileHandler config = new ConfigFileHandler();

        bool isConnected = true;
        while (isConnected)
        {

            string message = await appInfo.Client.ReceiveMessage();

            ConverterContainer cc; 
            try
            {
               cc = JsonSerializer.Deserialize<ConverterContainer>(message);

            }
            catch
            {
                cc = new ConverterContainer("exit_success",""); 
            }

            //Check what kind of message the server sent and act accordingly.
            switch (cc.Type)
            {
                case "Data":
                    //Update online users
                    appInfo.OnlineUsers = JsonSerializer.Deserialize<Dictionary<int, string>>(cc.JSON);
                    break;
                case "message_success":
                    //Update history of the chat were message was sent.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_message_history", cc.JSON)));
                    break;
                case "message_failed":
                    MessageBox.Show("Sending message failed!", "Fail",MessageBoxButton.OK);
                    break;
                case "request_groups_success":
                    //Update list of chats
                    List<GroupData> groups = JsonSerializer.Deserialize<List<GroupData>>(cc.JSON);
                    ObservableCollection<Chat> chats = new();

                    if(groups.Where(x => x.group_id == appInfo.ChatUI.uc_message_list.ChatId).ToList().Count() != 1 && appInfo.ChatUI.uc_openchats.Chats.Count() >= 1)
                    {
                        appInfo.ChatUI.uc_message_list.Dispatcher.Invoke(() =>
                        {
                            appInfo.ChatUI.uc_message_list.LoadSelectedChat(appInfo.ChatUI.uc_openchats.Chats[0]);
                        });
                    }

                    foreach (GroupData gd in groups)
                    {
                        string realName = gd.group_name;
                        if (gd.group_id != 1)
                        {
                            Console.WriteLine("Group ID: " + gd.group_id);
                            int id1 = int.Parse(gd.group_name.Split("-")[0]);
                            int id2 = int.Parse(gd.group_name.Split("-")[1]);

                            realName = (id1 == appInfo.ID) ? appInfo.AllUsers.GetValueOrDefault(id2, "Unknown User!") : appInfo.AllUsers.GetValueOrDefault(id1, "Unknown User!");
                        }
                        Chat c = new();
                        c.ChatName = realName;
                        c.ChatId = gd.group_id;
                        c.Messages = new ObservableCollection<Message>();
                        chats.Add(c);
                    }

                    appInfo.ChatUI.Chats = chats;

                    if (appInfo.ChatUI.Chats.Where(chat => !chat.Visibility).Count() > 0)
                    {
                        break;
                    }

                    appInfo.ChatUI.LoadCurrentChatsIntoList();
                    
                    break;
                case "reload_page":
                    //Server requests user to reload current chat (new message arrived)
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_message_history", cc.JSON)));
                    break;
                case "request_message_history_success":
                    //Insert messages into chat view.
                    List<MessageServer> msgList = JsonSerializer.Deserialize<List<MessageServer>>(cc.JSON);
                    if(msgList.Count() <= 0)
                    {
                        break;
                    }
                    appInfo.ChatUI.uc_message_list.Dispatcher.Invoke(() =>
                    {
                        appInfo.ChatUI.uc_openchats.Chats.Where(x => x.ChatId == msgList[0].group_id).First().Messages.Clear();
                        foreach (MessageServer msg in msgList)
                        {
                            if (msg.user_id == appInfo.ID)
                            {
                                appInfo.ChatUI.uc_openchats.Chats.Where(x => x.ChatId == msg.group_id).First().Messages.Add(new Message(true, msg.message_text, "ME", msg.message_timestamp));
                            }
                            else
                            {
                                appInfo.ChatUI.uc_openchats.Chats.Where(x => x.ChatId == msg.group_id).First().Messages.Add(new Message(false, msg.message_text, appInfo.AllUsers.GetValueOrDefault<int, string>(msg.user_id, "Error! User not found"), msg.message_timestamp));

                            }
                        }
                    });
                    break;
                case "request_users_success":
                    //Update list of ALL users.
                    appInfo.AllUsers = JsonSerializer.Deserialize<Dictionary<int, string>>(cc.JSON);
                    if (appInfo.ChatUI != null)
                    {
                        appInfo.ChatUI.Dispatcher.Invoke(() =>
                        {
                            appInfo.ChatUI.OnlineUsers.Clear();
                            foreach (int id in appInfo.AllUsers.Keys)
                            {
                                User u = new User(appInfo.AllUsers.GetValueOrDefault(id));
                                u.Id = id;
                                if(u.Id != appInfo.ID)
                                {
                                    appInfo.ChatUI.OnlineUsers.Add(u);
                                }

                            }
                        });
                    }
                    break;
                case "reload_users":
                    //Server requests to reload all online users.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_online_users", cc.JSON)));
                    break;
                case "request_online_users_success":
                    //Load online users into appInfo instance.
                    appInfo.OnlineUsers = JsonSerializer.Deserialize<Dictionary<int, string>>(cc.JSON);
                    break;
                case "invalid_command":
                    //Server didn't recognize command
                    break;
                case "edit_success":
                    //Edit request by the user was successful.
                    ConfigFileHandler configFileHandler = new ConfigFileHandler();
                    configFileHandler.SaveIPAndUsernameInConfigFile(configFileHandler.LoadConfigValues().Item1, appInfo.OptionsPage.Username);
                    Console.WriteLine("Switch Case : edit_success");
                    MessageBox.Show("Edit successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    appInfo.OptionsPage.Clear_Pw_Input(); 
                    break;
                case "is_admin_success":
                    //Is_admin request returned true.
                    appInfo.IsAdmin = true;
                    break;
                case "is_admin_failed":
                    //Is admin request returned false,
                    appInfo.IsAdmin = false;
                    break;
                case "create_chat_success":
                    //Creating a chat was successful. Request groups from server again!
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_groups",appInfo.ID.ToString())));
                    break;
                case "create_chat_failed":
                    //Creating a chat has failed. Show message to user.
                    MessageBox.Show("Failed to create the chat! Please try again or contact administrator!", "Fail",MessageBoxButton.OK);
                    break;
                case "reload_all_users":
                    //Server requests client to reload users.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_users","")));
                    break;
                case "ban_user_success":
                    //Ban request to server was successful.
                    MessageBox.Show("Ban successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "ban_user_failed":
                    //Ban request to server has failed.
                    MessageBox.Show("Ban failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "unban_user_success":
                    //Unban request to server was successful.
                    MessageBox.Show("Unban successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "unban_user_failed":
                    //Unban request to server has failed.
                    MessageBox.Show("Unban failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "kick_user_success":
                    //Kick user request was successful
                    MessageBox.Show("Kick successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "kick_user_failed":
                    //Kick user request has failed
                    MessageBox.Show("Kick failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "reset_password_success":
                    //Resetting the password was successful
                    MessageBox.Show("Reset password successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "reset_password_failed":
                    //Resetting password has failed.
                    MessageBox.Show("Reset password failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "promote_user_success":
                    //Promoting user to admin role was successful
                    MessageBox.Show("Promote successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "promote_user_failed":
                    //Promoting user to admin role has failed
                    MessageBox.Show("Promote failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "demote_user_success":
                    //Demoting adming to user was successful
                    MessageBox.Show("Demote successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "demote_user_failed":
                    //Demoting admin to user has failed.
                    MessageBox.Show("Demote failed", "Fail", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "reload_groups":
                    //Server has requested client to reload groups.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize( new ConverterContainer("request_groups",appInfo.ID.ToString())));
                    break;
                case "delete_chat_success":
                    //Deleting a chat was successful. Reloading groups.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_groups", appInfo.ID.ToString())));
                    break;
                case "delete_chat_failed":
                    //Deleting a chat has failed. Show message to user!
                    MessageBox.Show("Deleting chat failed!","Fail", MessageBoxButton.OK);
                    break;
                case "exit_success":
                    //Server has closed connection. Inform user and redirect to login page.
                    MessageBox.Show("Connection lost! You will be logged out now!", "Connection lost!", MessageBoxButton.OK);

                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.Width = 700;
                        mainWindow.Height = 850;
                        mainWindow.Top = 0;
                        mainWindow.Left = 0;
                        mainWindow.MainFrame.Navigate(new Welcome_Window(mainWindow));
                    });
                    isConnected = false;
                    break;
                case "reload_admin":
                    //Server notifies user that their admin status may have changed and needs to be reloaded.
                    appInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("is_admin", appInfo.ID.ToString())));
                    break;
                default:
                    Console.WriteLine("Received unknown request");
                    break;
            }
            
        }
    }
}
