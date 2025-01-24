using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ServerApplication
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Configuration; 

    public class Server
    {
        private Socket _serverSocket;
        public static List<Users> _clients = new List<Users>();
        public static Dictionary<Socket, EncryptionKey> ClientKeys = new Dictionary<Socket, EncryptionKey>();
        private RSAParameters serverKeys;

        public RSAParameters Keys
        {
            get
            {
                return this.serverKeys;
            }
            private set { }
        }
        public void Start()
        {
            try
            {
                // load IPAddress and port number from the config file
                string ipAddress = ConfigurationManager.AppSettings.Get("ServerIPAddress");
                int port = 0;
                if (!int.TryParse(ConfigurationManager.AppSettings.Get("ServerPort"), out port))
                {
                    throw new ConfigurationErrorsException("Invalid port number in configuration.");
                }

                // Starts the server
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(ipAddress);
                _serverSocket.Bind(new IPEndPoint(ip, port));
                _serverSocket.Listen(10);

                Console.WriteLine("Server started...");

                RSA rsaKeys = RSA.Create(2048);
                rsaKeys.KeySize = 2048;
                this.serverKeys = rsaKeys.ExportParameters(true);

                while (true)
                {
                    // Accepts the clients
                    Socket clientSocket = _serverSocket.Accept();
                    Console.WriteLine("Client connected.");
                    Thread clientThread = new Thread(() => new ClientHandler(this).HandleClient(clientSocket));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}