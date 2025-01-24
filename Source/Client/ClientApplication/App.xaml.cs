using System.Configuration;
using System.Data;
using System.Text.Json;
using System.Windows;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            /*
            if (UserInfo.ListenThread != null)
            {
                UserInfo.ListenThread.Join(10);
            }
            if (UserInfo.Client != null)
            {
                ConverterContainer cc = new ConverterContainer("exit", "");
                await UserInfo.Client.SendMessage(JsonSerializer.Serialize(cc));
            }
            */
        }
    }

}
