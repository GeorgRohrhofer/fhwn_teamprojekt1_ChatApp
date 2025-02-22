﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientApplication
{
    /// <summary>
    /// Interaktionslogik für ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ObservableCollection<User> Users { get; set; } = [new("Georg"), new("Leo")];

        public ApplicationInfo AppInfo { get; set; }
        public ChatWindow(ApplicationInfo appInfo)
        {
            InitializeComponent();
            this.DataContext = this;
            this.AppInfo = appInfo;
        }
    }
}
