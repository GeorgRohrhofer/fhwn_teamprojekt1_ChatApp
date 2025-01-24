using ClientApplication.dialogs;
using ClientApplication.Pages;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace ClientApplication.user_controls
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class ChatMessageList : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ChatWindowPage chatWindowPage;

        private ObservableCollection<Message> messages;
        private ObservableCollection<User> usersInChat;
        private ObservableCollection<User> usersOnline;
        private string chatname;
        private int chatId;
        private DateTime lastMessageSent = DateTime.MinValue;

        private bool _autoscroll = true;

        public ApplicationInfo AppInfo { get; set; }
        public string ChatName
        {
            get
            {
                return this.chatname;
            }
            set
            {
                this.chatname = value;
                this.firePropertyChanged(nameof(this.ChatName));
            }
        }
        public int ChatId
        {
            get
            {
                return this.chatId;
            }
            set
            {
                this.chatId = value;
                this.firePropertyChanged(nameof(this.ChatId));
            }
        }

        public ObservableCollection<Message> Messages
        {
            get
            {
                return this.messages;
            }
            set
            {
                this.messages = value;

                this.firePropertyChanged(nameof(this.Messages));
            }
        }

        public ObservableCollection<User> UsersInChat
        {
            get
            {
                return this.usersInChat;
            }
            set
            {
                this.usersInChat = value;
                this.firePropertyChanged(nameof(this.UsersInChat));
            }
        }

        public ObservableCollection<User> UsersOnline
        {
            get
            {
                return this.usersOnline;
            }
            set
            {
                this.usersOnline = value;
                this.firePropertyChanged(nameof(this.UsersOnline));
            }
        }

        //public Dictionary<int, ObservableCollection<Message>> messageLists;

        public ObservableCollection<User> Users = new();

        public ChatMessageList()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;

            MessageHandler.messageList = this;
            this.DataContext = this;
        }

        public void LoadSelectedChat(Chat chat)
        {
            this.Messages = chat.Messages;

            this.Messages.CollectionChanged += OnNewMessageRecieved;

            this.ChatName = chat.ChatName;
            this.ChatId = chat.ChatId;

            ScrollViewer? scrollViewer = GetScrollViewer(lb_messages);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToBottom();
            }
        }

        protected void firePropertyChanged(string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            if (tb_send.Text.Equals(string.Empty))
                return;

            string text = tb_send.Text;

            if (DateTime.UtcNow.Subtract(this.lastMessageSent).TotalMilliseconds < 1000)
            {
                double waitingTime = ((double)DateTime.UtcNow.Subtract(this.lastMessageSent).Milliseconds / 1000.0);
                lbl_FeedbackMessage.Content = $"Please wait for another {waitingTime} seconds before you send again.";

                new Thread(new ThreadStart(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Thread.Sleep((int)(waitingTime * 1000));
                        this.lbl_FeedbackMessage.Content = "";
                    });
                })).Start();

                return;
            }

            this.lastMessageSent = DateTime.UtcNow;

            //sendMessage(text);
            SendMessageHandler.AttemptSendMessage(text, chatId, this.AppInfo);
            //Thread.Sleep(500);
            //AppInfo.Client.SendMessage(JsonSerializer.Serialize( new ConverterContainer("request_message_history",chatId.ToString())));
            


            //this.Messages.Add(new Message(true, tb_send.Text, "ME", DateTime.Now));
            tb_send.Text = string.Empty;
        }


        private void Img_options_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            EditChatWindow dialog = new EditChatWindow();
            Image source = sender as Image;

            // set position of settings window
            Point posInWindow = source!.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            dialog.Left = posInWindow.X - 250;
            dialog.Top = posInWindow.Y + 30;

            //Adding users to dialog

            if (this.chatWindowPage.uc_openchats.lb_chats.SelectedItems.Count != 0)
            {
                this.Users.Clear();

                // manual copy, ref copy cant be used here
                foreach (User user in this.UsersOnline)
                {
                    User userCopy = new User(user.Name)
                    {
                        Id = user.Id,
                        IsAdmin = user.IsAdmin,
                        IsSelected = user.IsSelected,
                    };

                    this.Users.Add(userCopy);
                }

                // find checked users
                foreach (User user in this.Users)
                {
                    foreach (User userInGroup in this.usersInChat)
                    {
                        if (user.Id == userInGroup.Id)
                        {
                            user.IsSelected = true;
                        }
                    }
                }

                // open dialog to configure chat changes (not if dialog discarded / failed)
                dialog.uc_onlineUsers.Users = this.Users;

                if (!(bool)dialog.ShowDialog())
                {
                    return;
                }

                // update existing user lists
                ObservableCollection<User> updatedUserList = new(dialog.uc_onlineUsers.Users.Where(user => user.IsSelected == true));

                this.UsersInChat = updatedUserList;

                this.chatWindowPage.Chats.Where(chat => chat.ChatId == this.chatId).First().Users =
                    updatedUserList;
            }
        }

        /// <summary>
        /// Scroll to Bottom
        /// </summary>
        private void SetNewScrollPosition()
        {
            if (!this._autoscroll)
                return;

            ScrollViewer? scrollViewer = GetScrollViewer(lb_messages);

            if(scrollViewer != null)
            {
                scrollViewer.ScrollToBottom();
            }
        }

        /// <summary>
        /// ScrollChanged Event.
        /// Is called if the user scrolls within the Messages.
        /// If the user is not at the bottom of the list, autoscrolling is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_messages_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer? scrollViewer = GetScrollViewer(lb_messages);

            if(scrollViewer != null)
            {
                _autoscroll = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;
                //Console.WriteLine(_autoscroll);
            }
        }

        /// <summary>
        /// Returns the ScrollViewer of a dependencyObject if a scrollViewer is contained
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Scroll Viewer</returns>
        private ScrollViewer? GetScrollViewer (DependencyObject obj)
        {
            if (obj is ScrollViewer)
                return (ScrollViewer)obj;

            //Find the ScrollViewer recursivly
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);  
                ScrollViewer? result = GetScrollViewer(child);

                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Set new position if a new message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewMessageRecieved(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetNewScrollPosition();
        }

        private void tb_send_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.tb_send.Text.Length == 500) 
            {
                this.lbl_FeedbackMessage.Content = "Messages are limited to 500 characters.";
            }
        }
    }
}
