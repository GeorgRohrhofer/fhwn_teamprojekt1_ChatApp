namespace ServerApplication
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    public class ClientHandler
    {
        public EncryptionKey keyData
        {
            get;
            set;
        }
        public Server ServerToServe
        {
            get;
            private set;
        }
        public ClientHandler(Server s)
        {
            this.keyData = null;
            this.ServerToServe = s;
        }

        /// <summary>
        /// Handles the client
        /// </summary>
        /// <param name="clientSocket"></param>
        public void HandleClient(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[200000000];
                while (true)
                {
                    if (!IsClientConnected(clientSocket))
                        break;


                    int received = clientSocket.Receive(buffer);
                    //Console.WriteLine("Received packet of size +"+received);
                    string message;
                    if (this.keyData != null)
                    {
                        byte[] actualInput = new byte[received];
                        for(int i = 0; i < received; i++)
                        {
                            actualInput[i] = buffer[i];
                        }
                        message = Convert.ToBase64String(actualInput);
                        //Console.WriteLine("Conversion to base 64 done!");
                        message = SecurityHandler.Decrypt(message, this.keyData);
                        //Console.WriteLine(message);
                    }
                    else
                    {
                        message = Encoding.UTF8.GetString(buffer, 0, received);
                    }

                    //Console.WriteLine("Done with decryption");
                    string response = MessageHandler.HandleClientMessage(clientSocket, message,this);
                    Console.WriteLine("Response: " + response);

                    //Console.WriteLine("Done with response");
                    if (response == "exit" || !IsClientConnected(clientSocket))
                        break;

                    if(this.keyData != null)
                    {
                        //Console.WriteLine("Encrypted response!");
                        //Console.WriteLine("Plaintext: "+ response);
                        response = SecurityHandler.Encrypt(response, this.keyData);
                        //Console.WriteLine("Encrypting...");

                        clientSocket.Send(Convert.FromBase64String(response));
                    }
                    else
                    {
                        clientSocket.Send(Encoding.UTF8.GetBytes(response));
                    }

                    //Console.WriteLine("Sent response!");
                }
                DisconnectClient(clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client error: " + ex.Message);
            }
        }

        /// <summary>
        /// Check if the client is connected
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <returns></returns>
        private bool IsClientConnected(Socket clientSocket)
        {
            try
            {
                return clientSocket.Connected && !clientSocket.Poll(10000, SelectMode.SelectRead);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Disconnect the client
        /// </summary>
        /// <param name="clientSocket"></param>
        private void DisconnectClient(Socket clientSocket)
        {
            // remove client from _clients list
            Server._clients.Remove(Server._clients.Find(x => x.Socket == clientSocket));

            if (clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            Console.WriteLine("Client disconnected.");
        }
    }
}