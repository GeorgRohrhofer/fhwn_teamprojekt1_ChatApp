namespace ServerApplication
{
    using ServerApplication.Model;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using static System.Net.Mime.MediaTypeNames;
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Application Starting...");
            Server server = new Server();
            server.Start();
        }
    }
}
