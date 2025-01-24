using ClientApplication.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ClientApplication.Converters;
using System.Text.Json;

namespace ClientApplication.user_controls
{
    /// <summary>
    /// Interaction logic for OpenChatsList.xaml
    /// </summary>
    public partial class OpenChatsList : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Chat SelectedChat { get; set; } = new Chat()
        {
            ChatName = "empty / initial"
        };

        public ChatWindowPage ChatWindowPage { get; set; } = null!;

        private ObservableCollection<Chat> chats;

        public ObservableCollection<Chat> Chats
        {
            get
            {
                return this.chats;
            }
            set
            {
                this.chats = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Chats)));
            }
        }

        public OpenChatsList()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.Chats = [];
        }

        public void LoadChats(ObservableCollection<Chat> chats)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Chats.Clear();
                foreach (Chat chat in chats)
                {
                    this.Chats.Add(chat);
                }
            });
        }

        private void Lb_chats_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox)!.SelectedItems.Count == 0)
            {
                // hide chat as no chat is selected 
                this.ChatWindowPage.uc_message_list.Visibility = Visibility.Hidden;

                // list is now empty
                return;
            }

            this.SelectedChat = ((sender as ListBox)!.SelectedItems[0] as Chat)!;

            this.ChatWindowPage.AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("request_message_history",this.SelectedChat.ChatId.ToString())));

            // init chat
            this.ChatWindowPage.uc_message_list.Visibility = Visibility.Visible;

            this.ChatWindowPage.uc_message_list.ChatId = this.SelectedChat.ChatId;

            this.ChatWindowPage.uc_message_list.UsersInChat = Util.CopyObservableUserCollection(this.SelectedChat.Users);
            this.ChatWindowPage.uc_message_list.UsersOnline = Util.CopyObservableUserCollection(this.ChatWindowPage.OnlineUsers);

            this.ChatWindowPage.uc_message_list.LoadSelectedChat(this.SelectedChat);
        }

        private void Bn_delete_chat_OnClick(object sender, RoutedEventArgs e)
        {
            // remove chat when delete button is clicked
            int chatId = int.Parse((sender as Button).Tag.ToString());

            this.ChatWindowPage.AppInfo.Client.SendMessage(JsonSerializer.Serialize(new ConverterContainer("delete_chat",chatId.ToString())));
            
            //Chat removedChat = this.ChatWindowPage.Chats.First(chat => chat.ChatId == chatId);

            //this.ChatWindowPage.Chats.Remove(removedChat);
        }
    }
}