using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    using Azure;
    using Microsoft.EntityFrameworkCore.Storage.Json;
    using Newtonsoft.Json;
    using ServerApplication.Model;
    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Security.Cryptography;
    using System.Text.Json;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public class UserHandler
    {
        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public string EditUser(Socket clientSocket, string argument)
        {
            // Converts JSON to UserData object
            ConverterContainer converterContainer = JsonSerializer.Deserialize<ConverterContainer>(argument);
            UserData userData = JsonSerializer.Deserialize<UserData>(converterContainer.JSON);
            DatabaseContext db = new DatabaseContext();

            // Checks if user exists
            User user = db.users.Where(u => u.username == userData.Name).FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("Edit failed.");
                return JsonSerializer.Serialize(new ConverterContainer("edit_failed", ((int)Codes.FAILED).ToString()));
            }

            byte[] salt = user.salt;
            byte[] hashedPW = SecurityHandler.GetHashedPW(userData.Password, salt);

            // Updates the user
            user.username = userData.Name;
            user.password = hashedPW;
            user.email = userData.Email;

            db.SaveChanges();

            // Returns the user the success
            Console.WriteLine("Edit success for " + userData.Name);
            return JsonSerializer.Serialize(new ConverterContainer("edit_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="argument"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string Edit(Socket clientSocket, string argument, ClientHandler handler)
        {
            // Converts JSON to UserData object
            //ConverterContainer converterContainer = JsonSerializer.Deserialize<ConverterContainer>(argument);
            UserData userData = JsonSerializer.Deserialize<UserData>(argument);
            DatabaseContext db = new DatabaseContext();

            // Checks if user exists
            int userID = Server._clients.Where(c => c.Socket==clientSocket).First().Id;
            User user = db.users.Where(u => u.user_id == userID).FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("Edit failed.");
                return JsonSerializer.Serialize(new ConverterContainer("edit_failed", ((int)Codes.FAILED).ToString()));
            }

            byte[] salt = user.salt;
            byte[] hashedPW = SecurityHandler.GetHashedPW(userData.Password, salt);

            try
            {
                // Updates the user
                if (userData.Password == string.Empty)
                {
                    user.username = userData.Name;
                }
                else
                {
                    user.username = userData.Name;
                    user.password = hashedPW;
                }

                db.SaveChanges();

                foreach (Users cUser in Server._clients)
                {
                    cUser.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_users", "")), Server.ClientKeys.GetValueOrDefault(cUser.Socket))));
                }

                // edit user in _clients
                Server._clients.Where(c => c.Socket == clientSocket).First().Name = userData.Name;

            }
            catch (Exception e)
            {
                Console.WriteLine("Edit failed.");
                return JsonSerializer.Serialize(new ConverterContainer("edit_failed", ((int)Codes.FAILED).ToString()));
            }
            // Returns the user the success
            Console.WriteLine("Edit success for " + userData.Name);
            return JsonSerializer.Serialize(new ConverterContainer("edit_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Exits the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string Exit(Socket clientSocket, ClientHandler handler)
        {
            // Removes the client from the list
            Server._clients.Remove(Server._clients.Find(x => x.Socket == clientSocket));
            Server.ClientKeys.Remove(clientSocket);
            Dictionary<int, string> users = new Dictionary<int, string>();

            foreach (Users user in Server._clients)
            {
                users.Add(user.Id, user.Name);
            }

            ConverterContainer chatdata = new ConverterContainer("Data", JsonSerializer.Serialize(users));

            string sending = JsonSerializer.Serialize(chatdata);
            // Sends every client the new list of users
            foreach (Users user in Server._clients)
            {
                if (user.Socket != clientSocket)
                {
                    Console.WriteLine("Sending data to: " + user.Name);
                    user.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(sending, Server.ClientKeys.GetValueOrDefault(user.Socket))));
                }
            }

            // Returns the client the success
            return JsonSerializer.Serialize(new ConverterContainer("exit_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Requests the message history.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string RequestMessageHistory(Socket clientSocket, string augments, ClientHandler handler)
        {
            Console.WriteLine("test");
            // Check client id
            int id = 0;
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            id = user.Id;
            Console.WriteLine("Client id: " + id);

            // check if client is in group
            DatabaseContext databaseContext = new DatabaseContext();
            IQueryable<UserGroupMember> userGroups = databaseContext.usergroupmembers.Where(gu => gu.user_id == id && gu.group_id == Convert.ToInt32(augments));
            if (userGroups.Count() == 0)
            {
                Console.WriteLine("Client is not in group.");
                return JsonSerializer.Serialize(new ConverterContainer("request_message_history_failed", ((int)Codes.NOTINGROUP).ToString()));
            }

            // Send messages
            Console.WriteLine("Messages successfully returned.");
            // Only get the last 25 messages
            IQueryable<Message> messages = databaseContext.messages.Where(m => m.group_id == Convert.ToInt32(augments)).OrderByDescending(m => m.message_id).Take(25);
            List<Message> messageList = messages.ToList();
            messageList.Reverse();
            Console.WriteLine(JsonSerializer.Serialize(messageList));
            return JsonSerializer.Serialize(new ConverterContainer("request_message_history_success", JsonSerializer.Serialize(messageList)));
        }

        /// <summary>
        /// Requests the users.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string RequestUsers(Socket clientSocket, string augments, ClientHandler handler)
        {
            // get all users
            DatabaseContext databaseContext = new DatabaseContext();
            IQueryable<User> users = databaseContext.users;

            Dictionary<int, string> userList = new Dictionary<int, string>();

            foreach (User user in users)
            {
                userList.Add(user.user_id, user.username);
            }

            // return users
            Console.WriteLine("Users successfully returned.");
            Console.WriteLine(JsonSerializer.Serialize(userList));
            return JsonSerializer.Serialize(new ConverterContainer("request_users_success", JsonSerializer.Serialize(userList)));
        }

        /// <summary>
        /// Requests the online users.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string RequestOnlineUsers(Socket clientSocket, string augments, ClientHandler handler)
        {
            // get all online users
            Dictionary<int, string> users = new Dictionary<int, string>();
            foreach (Users user in Server._clients)
            {
                users.Add(user.Id, user.Name);
            }
            // return online users
            Console.WriteLine("Online users successfully returned.");
            Console.WriteLine(JsonSerializer.Serialize(users));
            return JsonSerializer.Serialize(new ConverterContainer("request_online_users_success", JsonSerializer.Serialize(users)));
        }

        /// <summary>
        /// Bans the users.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string BanUser(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            DatabaseContext databaseContext = new DatabaseContext();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // check if id is ids
            foreach (int id in ids)
            {
                if (user.Id == id)
                {
                    Console.WriteLine("Cannot ban yourself");
                    return JsonSerializer.Serialize(new ConverterContainer("ban_user_failed", ((int)Codes.FAILED).ToString()));
                }
            }

            // ban users
            try
            {
                databaseContext = new DatabaseContext();
                foreach (int id in ids)
                {
                    User users = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                    users.is_banned = true;
                }
                databaseContext.SaveChanges();

                // kick banned users
                foreach (int id in ids)
                {
                    Users users = Server._clients.Where(u => u.Id == id).FirstOrDefault();
                    if (users != null)
                    {
                        Server._clients.Remove(users);
                        users.Socket.Shutdown(SocketShutdown.Both);
                        users.Socket.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ban failed.");
                return JsonSerializer.Serialize(new ConverterContainer("ban_user_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Users successfully banned.");
            return JsonSerializer.Serialize(new ConverterContainer("ban_user_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string ResetPassword(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            DatabaseContext databaseContext = new DatabaseContext();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // check if id is ids
            foreach (int id in ids)
            {
                if (user.Id == id)
                {
                    Console.WriteLine("Cannot reset yourself");
                    return JsonSerializer.Serialize(new ConverterContainer("reset_password_failed", ((int)Codes.FAILED).ToString()));
                }
            }

            // reset passwords
            try
            {
                databaseContext = new DatabaseContext();
                foreach (int id in ids)
                {
                    User users = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                    byte[] salt = users.salt;
                    byte[] hashedPW = SecurityHandler.GetHashedPW(users.username + users.email, salt);
                    users.password = hashedPW;
                }
                databaseContext.SaveChanges();

                // kick users
                foreach (int id in ids)
                {
                    Users users = Server._clients.Where(u => u.Id == id).FirstOrDefault();
                    if (users != null)
                    {
                        Server._clients.Remove(users);
                        users.Socket.Shutdown(SocketShutdown.Both);
                        users.Socket.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Reset failed.");
                return JsonSerializer.Serialize(new ConverterContainer("reset_password_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Passwords successfully reset.");
            return JsonSerializer.Serialize(new ConverterContainer("reset_password_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Unbans the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string UnbanUser(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            DatabaseContext databaseContext = new DatabaseContext();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // check if id is ids
            foreach (int id in ids)
            {
                if (user.Id == id)
                {
                    Console.WriteLine("Cannot unban yourself");
                    return JsonSerializer.Serialize(new ConverterContainer("unban_user_failed", ((int)Codes.FAILED).ToString()));
                }
            }

            // unban users
            try
            {
                databaseContext = new DatabaseContext();
                foreach (int id in ids)
                {
                    User users = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                    users.is_banned = false;
                }
                databaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unban failed.");
                return JsonSerializer.Serialize(new ConverterContainer("unban_user_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Users successfully unbanned.");
            return JsonSerializer.Serialize(new ConverterContainer("unban_user_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Kicks the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string KickUser(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            DatabaseContext databaseContext = new DatabaseContext();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // check if id is ids
            foreach (int id in ids)
            {
                if (user.Id == id)
                {
                    Console.WriteLine("Cannot kick yourself");
                    return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
                }
            }

            // kick users
            try
            {
                foreach (int id in ids)
                {
                    Users users = Server._clients.Where(u => u.Id == id).FirstOrDefault();
                    Server._clients.Remove(users);
                    users.Socket.Shutdown(SocketShutdown.Both);
                    users.Socket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Kick failed.");
                return JsonSerializer.Serialize(new ConverterContainer("kick_user_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Users successfully kicked.");
            return JsonSerializer.Serialize(new ConverterContainer("kick_user_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Pomotes the user to admin.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string PromoteUser(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin in database
            DatabaseContext databaseContext = new DatabaseContext();
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("promote_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // ceck if ids has id
            foreach (int id in ids)
            {
                if (user.Id == id)
                {
                    Console.WriteLine("Cannot promote yourself.");
                    return JsonSerializer.Serialize(new ConverterContainer("promote_user_failed", ((int)Codes.FAILED).ToString()));
                }
            }



            // promote users
            try
            {
                databaseContext = new DatabaseContext();
                foreach (int id in ids)
                {
                    User users = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                    users.privilege_id = 1;
                }
                databaseContext.SaveChanges();

                // send to all online changed users to reload
                foreach (int id in ids)
                {
                    Users users = Server._clients.Where(u => u.Id == id).FirstOrDefault();
                    if (users != null)
                    {
                        users.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_admin", "")), Server.ClientKeys.GetValueOrDefault(users.Socket))));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Promote failed.");
                return JsonSerializer.Serialize(new ConverterContainer("promote_user_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Users successfully promoted.");
            return JsonSerializer.Serialize(new ConverterContainer("promote_user_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Demotes the user to normal user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string DemoteUser(Socket clientSocket, string augments, ClientHandler handler)
        {
            List<int> ids = JsonSerializer.Deserialize<List<int>>(augments);
            // check if clientsocket is admin in database
            DatabaseContext databaseContext = new DatabaseContext();
            Users user = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            User user1 = databaseContext.users.Where(u => u.user_id == user.Id).FirstOrDefault();
            if (user1.privilege_id != 1)
            {
                Console.WriteLine("Client is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("demote_user_failed", ((int)Codes.FAILED).ToString()));
            }

            // ceck if ids has id
            foreach (int id in ids)
            {
                if(user.Id == id)
                {
                    Console.WriteLine("Cannot demote yourself.");
                    return JsonSerializer.Serialize(new ConverterContainer("demote_user_failed", ((int)Codes.FAILED).ToString()));
                }
            }



            // promote users
            try
            {
                databaseContext = new DatabaseContext();
                foreach (int id in ids)
                {
                    User users = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                    users.privilege_id = 2;
                }
                databaseContext.SaveChanges();

                // send to all online changed users to reload
                foreach (int id in ids)
                {
                    Users users = Server._clients.Where(u => u.Id == id).FirstOrDefault();
                    if (users != null)
                    {
                        users.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_admin", "")), Server.ClientKeys.GetValueOrDefault(users.Socket))));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Demote failed.");
                return JsonSerializer.Serialize(new ConverterContainer("demote_user_failed", ((int)Codes.FAILED).ToString()));
            }
            Console.WriteLine("Users successfully demoted.");
            return JsonSerializer.Serialize(new ConverterContainer("demote_user_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Checks if user is admin.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string IsAdmin(Socket clientSocket, string augments, ClientHandler handler)
        {
            // get user id
            int id = Convert.ToInt32(augments);
            // get user
            DatabaseContext databaseContext = new DatabaseContext();
            User user = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
            // check if user is admin
            if (user.privilege_id == 1)
            {
                Console.WriteLine("User is admin.");
                return JsonSerializer.Serialize(new ConverterContainer("is_admin_success", ((int)Codes.SUCCESS).ToString()));
            }
            else
            {
                Console.WriteLine("User is not admin.");
                return JsonSerializer.Serialize(new ConverterContainer("is_admin_failed", ((int)Codes.FAILED).ToString()));
            }
        }

        /// <summary>
        /// Deletes the chat.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string DeleteChat(Socket clientSocket, string augments, ClientHandler handler)
        {
            int groupid = Convert.ToInt32(augments);
            // check if group id is not 1
            if (groupid == 1)
            {
                Console.WriteLine("Cannot delete group 1.");
                return JsonSerializer.Serialize(new ConverterContainer("delete_chat_failed", ((int)Codes.FAILED).ToString()));
            }
            int id = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault().Id;
            // check if client is in group
            DatabaseContext databaseContext = new DatabaseContext();
            IQueryable<UserGroupMember> userGroups = databaseContext.usergroupmembers.Where(gu => gu.user_id == id && gu.group_id == groupid);
            if (userGroups.Count() == 0)
            {
                Console.WriteLine("Client is not in group.");
                return JsonSerializer.Serialize(new ConverterContainer("delete_chat_failed", ((int)Codes.NOTINGROUP).ToString()));
            }
            Console.WriteLine("Client is in group.");
            // get other user in group
            IQueryable<UserGroupMember> groupMembers = databaseContext.usergroupmembers.Where(gm => gm.group_id == groupid && gm.user_id != id);
            int otherUser = groupMembers.FirstOrDefault().user_id;  
            Console.WriteLine("Other user: " + otherUser);
            try
            {
                // messages with groupid
                IQueryable<Message> messages = databaseContext.messages.Where(m => m.group_id == groupid);
                foreach (Message message in messages)
                {
                    databaseContext.messages.Remove(message);
                }
                databaseContext.SaveChanges();
                databaseContext = new DatabaseContext();

                // group members with groupid
                groupMembers = databaseContext.usergroupmembers.Where(gm => gm.group_id == groupid);
                foreach (UserGroupMember groupMember in groupMembers)
                {
                    databaseContext.usergroupmembers.Remove(groupMember);
                }
                databaseContext.SaveChanges();
                databaseContext = new DatabaseContext();

                // group with groupid
                UserGroup group = databaseContext.usergroups.Where(g => g.group_id == groupid).FirstOrDefault();
                databaseContext.usergroups.Remove(group);

                databaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Chat deletion failed.");
                return JsonSerializer.Serialize(new ConverterContainer("delete_chat_failed", ((int)Codes.FAILED).ToString()));
            }
            // reload groups for other user
            Users user2_2 = Server._clients.Where(x => x.Id == otherUser).FirstOrDefault();
            if (user2_2 != null)
            {
                user2_2.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_groups", "")), Server.ClientKeys.GetValueOrDefault(user2_2.Socket))));
            }
            
            return JsonSerializer.Serialize(new ConverterContainer("delete_chat_success", ((int)Codes.SUCCESS).ToString()));
        }

        /// <summary>
        /// Creates the chat.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string CreateChat(Socket clientSocket, string augments, ClientHandler handler)
        {
            try
            {
                int id = Convert.ToInt32(augments);
                DatabaseContext databaseContext = new DatabaseContext();
                // get username with id and clientSocket
                Users user1 = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
                User user2 = databaseContext.users.Where(u => u.user_id == id).FirstOrDefault();
                Console.WriteLine("User1: " + user1.Name);
                Console.WriteLine("User2: " + user2.username);

                // check if group already exists with both users
                IQueryable<UserGroupMember> userGroups1 = databaseContext.usergroupmembers.Where(gu => gu.user_id == user1.Id);
                IQueryable<UserGroupMember> userGroups2 = databaseContext.usergroupmembers.Where(gu => gu.user_id == user2.user_id);
                IQueryable<UserGroup> groups1 = databaseContext.usergroups.Where(g => userGroups1.Any(gu => gu.group_id == g.group_id));
                IQueryable<UserGroup> groups2 = databaseContext.usergroups.Where(g => userGroups2.Any(gu => gu.group_id == g.group_id));
                IQueryable<UserGroup> groups = groups1.Where(g => groups2.Any(g2 => g2.group_id == g.group_id && g2.group_id != 1));
                if (groups.Count() != 0)
                {
                    Console.WriteLine("Group already exists.");
                    return JsonSerializer.Serialize(new ConverterContainer("create_chat_failed", ((int)Codes.FAILED).ToString()));
                }

                // create chat

                UserGroup group = new UserGroup
                {
                    group_name = user1.Id + "-" + user2.user_id,
                };

                databaseContext.usergroups.Add(group);
                databaseContext.SaveChanges();
                databaseContext = new DatabaseContext();

                UserGroupMember groupMember1 = new UserGroupMember
                {
                    group_id = group.group_id,
                    user_id = user1.Id,
                };

                UserGroupMember groupMember2 = new UserGroupMember
                {
                    group_id = group.group_id,
                    user_id = user2.user_id,
                };

                Console.WriteLine("Group created.");
                
                databaseContext.usergroupmembers.Add(groupMember1);
                databaseContext.usergroupmembers.Add(groupMember2);
                databaseContext.SaveChanges();

                // reload page for user2
                Users user2_2 = Server._clients.Where(x => x.Id == user2.user_id).FirstOrDefault();
                if (user2_2 != null)
                {
                    user2_2.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_groups", "")), Server.ClientKeys.GetValueOrDefault(user2_2.Socket))));
                }
                return JsonSerializer.Serialize(new ConverterContainer("create_chat_success", ((int)Codes.SUCCESS).ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine("Chat creation failed.");
                return JsonSerializer.Serialize(new ConverterContainer("create_chat_failed", ((int)Codes.FAILED).ToString()));
            }
        }

        /// <summary>
        /// Requests the groups.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string RequestGroups(Socket clientSocket, string augments, ClientHandler handler)
        {
            // get all groups user is part of
            int id = Convert.ToInt32(augments);
            DatabaseContext databaseContext = new DatabaseContext();
            IQueryable<UserGroupMember> userGroups = databaseContext.usergroupmembers.Where(gu => gu.user_id == id);
            IQueryable<UserGroup> groups = databaseContext.usergroups.Where(g => userGroups.Any(gu => gu.group_id == g.group_id));
            List<UserGroup> groupList = groups.ToList();


            // return groups
            Console.WriteLine("Groups successfully returned.");
            Console.WriteLine(JsonSerializer.Serialize(groupList));
            return JsonSerializer.Serialize(new ConverterContainer("request_groups_success", JsonSerializer.Serialize(groupList)));
        }

        /// <summary>
        /// Login the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string Login(Socket clientSocket, string augments, ClientHandler handler)
        {
            DatabaseContext db = new DatabaseContext();
            UserData userData = JsonSerializer.Deserialize<UserData>(augments);

            User user = db.users.Where(u => (u.username == userData.Name || u.email == userData.Email)).FirstOrDefault();

            // Returns the user the Failure
            //Console.WriteLine("Returns the user the Failure");
            if (user == null)
            {
                Console.WriteLine("Login failed for " + userData.Name);
                return JsonSerializer.Serialize(new ConverterContainer("login_failed", ((int)Codes.FAILED).ToString()));
            }

            // encrypts the password
            Console.WriteLine("encrypts the password");
            byte[] salt = user.salt;
            byte[] hashedPW = SecurityHandler.GetHashedPW(userData.Password, salt);
            user = null;
            // Checks if user exists
            Console.WriteLine("Checks if user exists");
            user = db.users.Where(u => (u.username == userData.Name || u.email == userData.Email) && u.password == hashedPW).FirstOrDefault();

            // Returns the user the Failure
            //Console.WriteLine("Returns the user the Failure");
            if (user == null)
            {
                Console.WriteLine("Login failed for " + userData.Name);
                return JsonSerializer.Serialize(new ConverterContainer("login_failed", ((int)Codes.FAILED).ToString()));
            }
            string sending = "";

            // check if user is banned
            Console.WriteLine("check if user is banned");
            if (user.is_banned == true)
            {
                Console.WriteLine("User is banned.");
                return JsonSerializer.Serialize(new ConverterContainer("login_failed", ((int)Codes.BANNED).ToString()));
            }


            // Removes client if he is connected with another account
            Console.WriteLine("Removes client if he is connected with another account");
            Users user1 = Server._clients.Where(x => x.Id == user.user_id).FirstOrDefault();
            if (user1 != null)
            {
                Server._clients.Remove(user1);
                user1.Socket.Shutdown(SocketShutdown.Both);
                user1.Socket.Close();
            }

            // Adds the client to the list
            Console.WriteLine("Adds the client to the list");
            Server._clients.Add(new Users(user.user_id, user.username, clientSocket));

            Dictionary<int, string> users = new Dictionary<int, string>();



            foreach (Users cUser in Server._clients)
            {
                users.Add(cUser.Id, cUser.Name);
            }

            ConverterContainer chatdata = new ConverterContainer("Data", JsonSerializer.Serialize(users));

            sending = JsonSerializer.Serialize(chatdata);

            // Sends every client the new list of users
            Console.WriteLine("Sends every client the new list of users");
            foreach (Users cUser in Server._clients)
            {
                if (cUser.Socket != clientSocket)
                {
                    Console.WriteLine("Sending data to: " + cUser.Name);
                    cUser.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(sending, Server.ClientKeys.GetValueOrDefault(cUser.Socket))));
                }
            }

            // Sends the client the list and success
            string returnString = JsonSerializer.Serialize(users);
            returnString = JsonSerializer.Serialize(new ConverterContainer(user.user_id.ToString(), returnString));
            returnString = JsonSerializer.Serialize(new ConverterContainer("login_success", returnString));
            Console.WriteLine("Login success for " + user.username);
            return returnString;
        }

        /// <summary>
        /// Sends messages in Group.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string SendMessage(Socket clientSocket, string augments, ClientHandler handler)
        {
            Console.WriteLine(augments);
            ConverterContainer converterContainer = JsonSerializer.Deserialize<ConverterContainer>(augments);

            int id = 0;
            Users tUser = Server._clients.Where(x => x.Socket == clientSocket).FirstOrDefault();
            id = tUser.Id;

            int groupid = Convert.ToInt32(converterContainer.Type);

            // check if client is in group
            DatabaseContext databaseContext = new DatabaseContext();
            IQueryable<UserGroupMember> userGroups = databaseContext.usergroupmembers.Where(gu => gu.user_id == id && gu.group_id == groupid);
            if (userGroups.Count() == 0)
            {
                Console.WriteLine("Client is not in group.");
                return JsonSerializer.Serialize(new ConverterContainer("message_failed", ((int)Codes.NOTINGROUP).ToString()));
            }

            // Adds the message to the database
            Message message = new Message
            {
                group_id = groupid,
                user_id = id,
                message_text = converterContainer.JSON,
                message_timestamp = DateTime.UtcNow,
            };

            databaseContext.messages.Add(message);
            databaseContext.SaveChanges();

            // get all clients in group
            userGroups = databaseContext.usergroupmembers.Where(gu => gu.group_id == groupid);
            IQueryable<User> users = databaseContext.users.Where(u => userGroups.Any(gu => gu.user_id == u.user_id));

            foreach(User user in users) {
                foreach(Users cUser in Server._clients)
                {
                    if (cUser.Id == user.user_id && tUser.Id != cUser.Id)
                    {
                        Console.WriteLine("Sending message to: " + cUser.Name);
                        ConverterContainer responds = new ConverterContainer("reload_page", groupid.ToString());
                        cUser.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(responds), Server.ClientKeys.GetValueOrDefault(cUser.Socket))));
                    }
                }
            }



            // Sends the client the success
            Console.WriteLine("Message Successfully send.");
            return JsonSerializer.Serialize(new ConverterContainer("message_success", converterContainer.Type));
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="augments"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public string Register(Socket clientSocket, string augments, ClientHandler handler)
        {
            Console.WriteLine(augments);
            UserData userData = JsonSerializer.Deserialize<UserData>(augments);
            DatabaseContext db = new DatabaseContext();

            // Checks if there is an empty symbol
            foreach (char c in userData.Name)
            {
                if (c == ' ')
                {
                    Console.WriteLine("Username contains empty symbol.");
                    return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.EMPTYSYMBOL).ToString()));
                }
            }

            // Checks if email is valid
            if (!userData.Email.Contains('@') || !userData.Email.Contains('.'))
            {
                Console.WriteLine("Email is not valid.");
                return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.INVALIDEMAIL).ToString()));
            }

            // Checks if password is valid
            if (userData.Password.Length < 8)
            {
                Console.WriteLine("Password is too short.");
                return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.PASSWORDTOOSHORT).ToString()));
            }

            // Checks if username or email is already taken
            IQueryable<User> userselect = db.users.Where(u => u.username == userData.Name);
            if (userselect.Count() > 0)
            {
                Console.WriteLine("Username already taken.");
                return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.USERNAMEALREADYTAKEN).ToString()));
            }
            userselect = db.users.Where(u => u.email == userData.Email);
            if (userselect.Count() > 0)
            {
                Console.WriteLine("Email already taken.");
                return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.EMAILALREADYTAKEN).ToString()));
            }

            // encrypts the password
            byte[] salt = new byte[100];
            Random rand = new Random();
            rand.NextBytes(salt);
            byte[] hashedPW = SecurityHandler.GetHashedPW(userData.Password, salt);

            // Adds the user to the database
            User user = new User
            {
                username = userData.Name,
                password = hashedPW,
                email = userData.Email,
                is_banned = false,
                salt = salt,
                reset_pw = false,
                privilege_id = 2
            };

            // Returns the user the failure
            if (user == null)
            {
                Console.WriteLine("Register failed.");
                return JsonSerializer.Serialize(new ConverterContainer("register_failed", ((int)Codes.FAILED).ToString()));
            }

            db.users.Add(user);
            db.SaveChanges();

            // send to all clients reload_all_users
            foreach (Users cUser in Server._clients)
            {
                cUser.Socket.Send(Convert.FromBase64String(SecurityHandler.Encrypt(JsonSerializer.Serialize(new ConverterContainer("reload_all_users", "")), Server.ClientKeys.GetValueOrDefault(cUser.Socket))));
            }

            // Returns the user the success
            Console.WriteLine("Register success for " + userData.Name);
            return JsonSerializer.Serialize(new ConverterContainer("register_success", ((int)Codes.SUCCESS).ToString()));
        }

        //Handle key exchange to setup encrypted connection.
        public string InitKeyExchange(Socket clientSocket, string augments, ClientHandler handler)
        {
            //Console.WriteLine(augments);
            RSAParameters clientPublicKey = JsonConvert.DeserializeObject<RSAParameters>(augments);

            RSA encryptor = RSA.Create(2048);
            encryptor.KeySize = 2048;
            encryptor.ImportParameters(clientPublicKey);


            RSA serverData = RSA.Create(2048);
            serverData.KeySize = 2048;
            //Console.WriteLine(handler.ServerToServe.Keys);
            serverData.ImportParameters(handler.ServerToServe.Keys);

            ConverterContainer response = new ConverterContainer("received_key",JsonConvert.SerializeObject(serverData.ExportParameters(false)));

            //Console.WriteLine("Preparing to send received key...");
            clientSocket.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize( response)));
            //Console.WriteLine("Received key message sent!");
            byte[] buffer = new byte[4096];
            //Console.WriteLine("Preparing to receive answer...");
            int answerSize = clientSocket.Receive(buffer);
            //Console.WriteLine("Received answer!");

            string message = Encoding.UTF8.GetString(buffer, 0, answerSize);

            //Console.WriteLine(message);
            response = JsonSerializer.Deserialize<ConverterContainer>(message);
            if(response.Type == "send_symmetric_key")
            {
                //Console.WriteLine("Received symmetric key!");
                EncryptionKey remoteKey = JsonSerializer.Deserialize<EncryptionKey>(response.JSON);

                Aes symetricEncryptor = Aes.Create();
                symetricEncryptor.KeySize = 256;
                symetricEncryptor.GenerateIV();
                symetricEncryptor.GenerateKey();

                EncryptionKey localKey = new EncryptionKey(symetricEncryptor.Key, symetricEncryptor.IV);

                //Console.WriteLine("Localsize:" + localKey.key.Length);
                //Console.WriteLine("Localsize:" + symetricEncryptor.Key.Length);
                byte[] encryptedKey = encryptor.Encrypt(symetricEncryptor.Key, RSAEncryptionPadding.Pkcs1);
                byte[] encryptedIV = encryptor.Encrypt(symetricEncryptor.IV, RSAEncryptionPadding.Pkcs1);

                //Console.WriteLine("Encrypted size: "+ encryptedKey.Length);

                string encryptedMessage = JsonSerializer.Serialize( new ConverterContainer("received_symmetric_key", JsonSerializer.Serialize(new EncryptionKey(encryptedKey, encryptedIV))));

                //Console.WriteLine(encryptedMessage);
                //Console.WriteLine("Preparing to send received_key...");
                remoteKey.key = serverData.Decrypt(remoteKey.key, RSAEncryptionPadding.Pkcs1);
                remoteKey.IV = serverData.Decrypt(remoteKey.IV, RSAEncryptionPadding.Pkcs1);

                handler.keyData = SecurityHandler.CombineKeys(localKey, remoteKey);

                clientSocket.Send(Encoding.UTF8.GetBytes(encryptedMessage));
                //Console.WriteLine("Testing...");
                Server.ClientKeys.Add(clientSocket, handler.keyData);
                return JsonSerializer.Serialize(new ConverterContainer("key_exchange_success", ""));

            }

            return JsonSerializer.Serialize(new ConverterContainer("key_exchange_failed",""));
        }
    }
}