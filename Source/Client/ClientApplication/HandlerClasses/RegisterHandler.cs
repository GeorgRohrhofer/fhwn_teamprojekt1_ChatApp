using ClientApplication.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class to handle registering.
    /// </summary>
    public class RegisterHandler
    {
        /// <summary>
        /// Attempts to register on the server with the given userinformation.
        /// </summary>
        /// <param name="ip">IP of the server.</param>
        /// <param name="email">Email of the user to register.</param>
        /// <param name="username">Username of the user to register.</param>
        /// <param name="password">Password of the user to register.</param>
        /// <returns></returns>
        public static async Task<RegisterReturnCodes> AttemptRegister(string ip, string email, string username, string password) 
        {
            try { 
                Client c = await Client.CreateClient(ip);
                UserData ud = new UserData();
                ud.Email = email;
                ud.Name = username;
                ud.Password = password;
                ConverterContainer cc = new ConverterContainer("register", JsonSerializer.Serialize(ud));
                await c.SendMessage(JsonSerializer.Serialize(cc));

                string reply = await c.ReceiveMessage();




                cc = JsonSerializer.Deserialize<ConverterContainer>(reply);

                if (cc == null)
                {
                    return RegisterReturnCodes.FAILURE;
                }


                RegisterReturnCodes returnCode = RegisterReturnCodes.FAILURE;

                returnCode = (RegisterReturnCodes)int.Parse(cc.JSON);
                return returnCode;
            }
            catch (ConnectionException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                return RegisterReturnCodes.FAILURE;
            }

        }
    }
}
