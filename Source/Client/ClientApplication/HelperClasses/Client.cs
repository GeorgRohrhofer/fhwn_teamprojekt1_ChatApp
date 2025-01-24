using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Runtime.CompilerServices;

namespace ClientApplication
{
    /// <summary>
    /// Represents a TCP client that connects to a server and handles communication using sockets.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Symmetrical key used for encrypting the connection.
        /// </summary>
        private EncryptionKey keyData;

        /// <summary>
        /// The current socket instance.
        /// </summary>
        private Socket currentSocket;

        /// <summary>
        /// Indicates whether the client is subscribed to updates.
        /// </summary>
        private bool isSubscribed;


        /// <summary>
        /// Async method to create client. Used to ensure key exchange is completed before any communication happens!
        /// </summary>
        /// <param name="ipAddr">IP Address to connect to</param>
        /// <returns></returns>
        public static async Task<Client> CreateClient(string ipAddr)
        {
            Client c = new Client(ipAddr);
            await c.ExchangeKeys();
            return c;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        private Client(string ipAddr)
        {
            keyData = null;
            isSubscribed = false;
            string ip = ipAddr;
            int port = 0;

            // get the values from the App.config file
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("ServerPort"), out port))
            {
                throw new ConfigurationErrorsException("Invalid port number in configuration.");
            }

            int retries;
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("Retries"), out retries))
            {
                throw new ConfigurationErrorsException("Invalid Retries number in configuration.");
            }

            int timeout;
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("Timeout"), out timeout))
            {
                throw new ConfigurationErrorsException("Invalid Timeout number in configuration.");
            }

            // try to connect to the server 
            while (retries > 0)
            {
                try
                {
                    currentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    currentSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
                   
                    Thread.Sleep(timeout);
                    if (!currentSocket.Connected)
                    {
                        throw new SocketException();
                    }
                    Console.WriteLine("Connected to server");



                    Task.Run(() => HandleCommunication());
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not connect to server: " + e.Message);
                    retries--;

                    if (retries > 0)
                    {
                        Console.WriteLine("Retrying connection...");
                        System.Threading.Thread.Sleep(timeout);
                    }
                }
            }

            throw new ConnectionException("Failed to connect after 5 attempts. \n Please check the server.");
        }

        /// <summary>
        /// Handles communication with the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleCommunication()
        {
            byte[] buffer = new byte[256];

            while (true)
            {
                string selectedOption = Console.ReadLine();

                if (selectedOption == "subscribe" && !isSubscribed)
                {
                    isSubscribed = true;
                    Console.WriteLine("Subscribed to updates");
                }

                await SendMessage(selectedOption);
                string response = await ReceiveMessage();
                //Console.WriteLine(response);
            }
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        /// <param name="message">The message text to send.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendMessage(string message)
        {
            Thread.Sleep(300);
            if(this.keyData != null)
            {

                //Console.WriteLine(message);
                message = SecurityHandler.Encrypt(message, this.keyData);
                byte[] data = Convert.FromBase64String(message);
                //Console.WriteLine("Sending message now... Size is "+ data.Length);

                currentSocket.Send(data);
                //Console.WriteLine("Sent request!");
                //await currentSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
            }
            else
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await currentSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
            }
        }

        /// <summary>
        /// Sends an unencrypted message to the server.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns></returns>
        private async Task SendMessageWithoutEncryption(string message)
        {
            if (currentSocket.Connected)
            {

                byte[] data = Encoding.ASCII.GetBytes(message);
                await currentSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
            }
            else
            {
                // Handle the case when the socket is not connected
                Console.WriteLine("Socket is not connected.");
            }
        }

        /// <summary>
        /// Receives a message from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the received message.</returns>
        public async Task<string> ReceiveMessage()
        {
            var buffer = new byte[20000000];
            int bytesRead = await currentSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            if(this.keyData != null)
            {
                byte[] actualInputSize = new byte[bytesRead];
                for (int i = 0; i< bytesRead; i++)
                {
                    actualInputSize[i] = buffer[i];
                } 
                return SecurityHandler.Decrypt(Convert.ToBase64String(actualInputSize), this.keyData);
            }

            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        

        
        public void SaveIPAndUsernameInConfigFile(string ipAddress, string username)
        {
            // get path to config file
            string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");

            // load config file
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFilePath
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            // check if key already exist and save the values 
            if (config.AppSettings.Settings["IpAddress"] == null)
            {
                config.AppSettings.Settings.Add("IpAddress", ipAddress);
            }
            else
            {
                config.AppSettings.Settings["IpAddress"].Value = ipAddress;
            }

            if (config.AppSettings.Settings["CurrentUser"] == null)
            {
                config.AppSettings.Settings.Add("CurrentUser", username);
            }
            else
            {
                config.AppSettings.Settings["CurrentUser"].Value = username;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        //returns the actual Username of the User, so the client can send a request to the server for editing user
        public string GetUsername_from_Config()
        {
            string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFilePath
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            return config.AppSettings.Settings["CurrentUser"].Value;
        }


        public void SetConfigUserName(string userName)
        {
            string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");

            // load config file
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFilePath
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            // check if key already exist and save the values 
            if (config.AppSettings.Settings["CurrentUser"] == null)
            {
                config.AppSettings.Settings.Add("CurrentUser", userName);
            }
            else
            {
                config.AppSettings.Settings["CurrentUser"].Value = userName;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Disconnects the client from the server and closes the socket.
        /// </summary>
        public void DisconnectToServer()
        {
            if (currentSocket != null && currentSocket.Connected)
            {
                try
                {
                    currentSocket.Shutdown(SocketShutdown.Both);
                    currentSocket.Close();
                    isSubscribed = false;
                    Console.WriteLine("Disconnected from server");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while disconnecting: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("Socket is already disconnected.");
            }
        }

        /// <summary>
        /// Attempts to exchange encryption keys with the connected server.
        /// </summary>
        private async Task ExchangeKeys()
        {
            //Prepare and send public key
            RSA keys = RSA.Create(2048);
            keys.KeySize = 2048;

            RSAParameters publicKey = keys.ExportParameters(false);
            ConverterContainer message = new ConverterContainer("init_key_exchange", JsonConvert.SerializeObject(publicKey));

            await this.SendMessageWithoutEncryption(JsonSerializer.Serialize(message));
            string response = await this.ReceiveMessage();


            //Check response
            message = JsonSerializer.Deserialize<ConverterContainer>(response);
            if(message.Type == "received_key")
            {
                //Import server public key.
                RSAParameters serverPubKey = JsonConvert.DeserializeObject<RSAParameters > (message.JSON);
                RSA rsaEncryptor = RSA.Create();
                rsaEncryptor.KeySize = 2048;
                rsaEncryptor.ImportParameters(serverPubKey);

                //Generate symmetric key.
                Aes symetricEncryptor = Aes.Create();
                symetricEncryptor.KeySize = 256;
                symetricEncryptor.GenerateIV();
                symetricEncryptor.GenerateKey();

                //Encrypte symmetric key
                byte[] encryptedKey = rsaEncryptor.Encrypt(symetricEncryptor.Key, RSAEncryptionPadding.Pkcs1);
                byte[] encryptedIV = rsaEncryptor.Encrypt(symetricEncryptor.IV, RSAEncryptionPadding.Pkcs1);

                //Send symmetric key.
                EncryptionKey localKey = new EncryptionKey(encryptedKey, encryptedIV);
                message = new ConverterContainer("send_symmetric_key",JsonSerializer.Serialize(localKey));
                await this.SendMessageWithoutEncryption(JsonSerializer.Serialize(message));
                response = await this.ReceiveMessage();

                //Check response.
                message = JsonSerializer.Deserialize<ConverterContainer>(response);
                if (message.Type == "received_symmetric_key")
                {
                    //Decrypt symmetric key from server.
                    EncryptionKey remoteKey = JsonSerializer.Deserialize<EncryptionKey>(message.JSON);
                    remoteKey.key = keys.Decrypt(remoteKey.key, RSAEncryptionPadding.Pkcs1);
                    remoteKey.IV = keys.Decrypt(remoteKey.IV, RSAEncryptionPadding.Pkcs1);


                    //Combine keys and accept dummy answer.
                    this.keyData = SecurityHandler.CombineKeys(new EncryptionKey(symetricEncryptor.Key, symetricEncryptor.IV), remoteKey);
                    string data = await this.ReceiveMessage();
                }
            }
            else
            {
                throw new ConnectionException("Key exchange failed!");
            }
        }

        /// <summary>
        /// Property to check if the client is still connected.
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return this.currentSocket.Connected;
        }
    }
}