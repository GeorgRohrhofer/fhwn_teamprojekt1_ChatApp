using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Runtime.CompilerServices;
using System.Text.Json;

using System.Windows.Controls;
using System.Windows.Input;
using ClientApplication.dialogs;
using ClientApplication.user_controls;

namespace ClientApplication.Pages
{
    /// <summary>
    /// Interaction logic for ChatWindowPage.xaml
    /// </summary>
    public partial class ChatWindowPage : Page, INotifyPropertyChanged
    {
        private Frame mainFrame;

        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<Chat> chats;

        

        public ObservableCollection<User> OnlineUsers { get; set; } = new()
        {
        };

        public ObservableCollection<Chat> Chats
        {
            get
            {
                return this.chats;
            }
            set
            {
                this.chats = value;
                OnPropertyChanged();
            }
        }

        public ApplicationInfo AppInfo { get; set; }
        public ChatWindowPage(Frame mainFrame, ApplicationInfo appInfo)
        {
            InitializeComponent();



            this.mainFrame = mainFrame;
            this.AppInfo = appInfo;

            this.uc_openchats.ChatWindowPage = this;

            this.uc_serachbar.ChatWindowPage = this;



            this.uc_message_list.chatWindowPage = this;
            this.uc_message_list.AppInfo = appInfo;
            appInfo.ChatUI = this;
            this.Chats = [
            ];
            foreach (int id in this.AppInfo.AllUsers.Keys)
            {
                User u = new User(this.AppInfo.AllUsers.GetValueOrDefault(id));
                u.Id = id;
                if(this.AppInfo.ID != id)
                {
                    this.OnlineUsers.Add(u);
                }
            }

            this.uc_message_list.ChatName = "test";

            this.AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_users","")));

            this.uc_message_list.AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_groups", this.uc_message_list.AppInfo.ID.ToString())));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Loads the chats from the Chat property into the OpenChatsList custom user control.
        /// </summary>
        public void LoadCurrentChatsIntoList()
        {
            this.uc_openchats.LoadChats(this.Chats);

            //this.uc_message_list.AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_groups", this.uc_message_list.AppInfo.ID.ToString())));
        }

        /// <summary>
        /// creates a new chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_plus_icon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateNewChatDialog dialog = new CreateNewChatDialog();
            Console.WriteLine("Total users: "+ this.OnlineUsers.Count);
            //dialog.uc_online_users.Users = Util.CopyObservableUserCollection(this.OnlineUsers);

            //add all users to dialiog
            dialog.uc_online_users.Users.Clear();
            //foreach (User u in this.OnlineUsers)
            //{
            //    dialog.uc_online_users.Users.Add(u);
            //}

            this.OnlineUsers.Where(user => user.Id != this.AppInfo.ID).ToList().ForEach(u => dialog.uc_online_users.Users.Add(u));

            // If user selected discard in dialog dont add a new chat
            if (!(bool)dialog.ShowDialog())
            {
                return;
            }

            // create new group
            //this.Chats.Add(new Chat()
            //{
            //    Users = new ObservableCollection<User>(dialog.uc_online_users.Users.Where(user => user.IsSelected)),
            //    ChatId = this.Chats.Count + 1,
            //    ChatName = dialog.tb_chat_name.Text,
            //    Messages = [new Message(false, "hello", "leo", DateTime.UtcNow)],
            //    CreationTime = DateTime.UtcNow,
            //});

            AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("create_chat", dialog.uc_online_users.Users.Where(user => user.IsSelected).First().Id.ToString())));


            //this.LoadCurrentChatsIntoList();
        }

        private void img_options_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.mainFrame.Navigate(new Options_Page(this.mainFrame, this, this.AppInfo));
        }
    }
}
