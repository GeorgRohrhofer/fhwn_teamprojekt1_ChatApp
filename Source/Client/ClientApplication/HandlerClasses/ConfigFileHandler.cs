using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Handles operations for reading and writing configuration values in the application configuration file.
    /// </summary>
    public class ConfigFileHandler
    {
        /// <summary>
        /// Saves the specified IP address and username in the configuration file.
        /// </summary>
        /// <param name="ipAddress">The IP address to save.</param>
        /// <param name="username">The username to save.</param>
        public void SaveIPAndUsernameInConfigFile(string ipAddress, string username)
        {
            try
            {
                // Get the path to the configuration file.
                string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");

                // Load the configuration file.
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFilePath
                };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                // Check if the keys already exist and save the values.
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

                // Save changes and refresh the configuration section.
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (ConfigurationErrorsException ex)
            {
                // Log the error or handle it as needed
                Console.WriteLine($"Error saving configuration: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access exception
                Console.WriteLine($"Unauthorized access: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the username from the configuration file.
        /// </summary>
        /// <returns>The current username stored in the configuration file.</returns>
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

        /// <summary>
        /// Sets the username in the configuration file.
        /// </summary>
        /// <param name="userName">The username to save.</param>
        public void SetConfigUserName(string userName)
        {
            string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");

            // Load the configuration file.
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFilePath
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            // Check if the key already exists and save the value.
            if (config.AppSettings.Settings["CurrentUser"] == null)
            {
                config.AppSettings.Settings.Add("CurrentUser", userName);
            }
            else
            {
                config.AppSettings.Settings["CurrentUser"].Value = userName;
            }

            // Save changes and refresh the configuration section.
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Loads the IP address and username values from the configuration file.
        /// </summary>
        /// <returns>A tuple containing the IP address and username, or null if they are not found.</returns>
        public Tuple<string?, string?> LoadConfigValues()
        {
            try
            {
                // Get the path to the configuration file.
                string configFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientApplication.dll.config");

                // Load the configuration file.
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFilePath
                };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                // Get the IP address and username from the configuration file.
                return Tuple.Create(config.AppSettings.Settings["IpAddress"]?.Value, config.AppSettings.Settings["CurrentUser"]?.Value);
            }
            catch (ConfigurationErrorsException ex)
            {
                // Log the error or handle it as needed
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                return Tuple.Create<string?, string?>(null, null);
            }
        }
    }
}
