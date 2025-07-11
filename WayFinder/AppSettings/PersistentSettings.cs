using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WayFinder.AppSettings
{
    /// <summary>
    /// Provides a singleton instance for managing persistent application settings related to project configurations.
    /// </summary>
    /// <remarks>The <see cref="PersistentSettings"/> class is designed to manage and persist project-specific
    /// settings,  determining whether each project is configured to run. It uses a singleton pattern to ensure a single
    /// instance is used throughout the application. The settings are stored in a JSON file located in the user's  local
    /// application data folder.</remarks>
    public sealed class PersistentSettings
    {
        /// <summary>
        /// Represents a collection of project settings that determine whether each project is configured to run.
        /// </summary>
        /// <remarks>This class provides a dictionary where the key is the project name and the value is a
        /// boolean indicating whether the application will run for that project. It is intended to be used for
        /// persisting project-specific settings.</remarks>
        private class PersistentData
        {
            // dictionary to be saved to settings file.
            // key = project name, val = whether or not the app will run
            public Dictionary<string, bool> Projects { get; set; } = new Dictionary<string, bool>();

        }

        // ========================================== FIELDS ======================================================================

        // the singleton _instance field 
        // private ensures the field can only be accessed from within the singleton class
        // static means that the field belongs to the class itself, not an _instance, hence there is only one
        // readonly ensures that _instance can only be assigned a value once. After the initial declaration, it cant be changed.
        private static readonly PersistentSettings _instance = new PersistentSettings();

        private PersistentData _settingsData; // the object that actually holds the settings during runtime

        private readonly string _settingsFilePath; // the file path of the persistent settings .json file


        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static PersistentSettings Instance => _instance;

        // ===================================== CONSTRUCTORS ====================================================================

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefield init
        // this ensures static fields are initialized exactly when the class is first accessed.
        static PersistentSettings()
        {
        }

        // the _instance constructor for the class
        // private ensure that code outside the class can't use 'new AppSettings'
        private PersistentSettings()
        {
            _settingsFilePath = GetSettingsFilePath();
            if (_settingsFilePath != null)
            {
                LoadSettings();
            }
            
        }

        // =====================================METHODS===========================================================================

        /// <summary>
        /// Updates the settings for a specified model by setting its active status.
        /// </summary>
        /// <remarks>This method requires a valid settings file path to function correctly. If the
        /// settings file path is not set, or if the model name is invalid, the method will display an error message and
        /// return <see langword="false"/>.</remarks>
        /// <param name="modelName">The name of the model whose settings are to be updated. Cannot be null or empty.</param>
        /// <param name="active">A boolean value indicating whether the model should be active. <see langword="true"/> to activate the model;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if the settings were successfully updated; otherwise, <see langword="false"/>.</returns>
        public bool SetSettings(string modelName, bool active)
        {
            if (_settingsFilePath == null)
            {
                TaskDialog.Show("Error", "WayFinder couldn't save this model's settings.\n" +
                    "Couldn't find settings file.");
                return false;
            }

            if (modelName == null || modelName == "")
            {
                TaskDialog.Show("Error", "WayFinder couldn't save this model's settings.\n" +
                    "Invalid Model Name.");
                return false;
            }

            _settingsData.Projects[modelName] = active;
            SaveSettings();
            return true;
        }
        
        
        /// <summary>
        /// Retrieves the settings value for the specified model name.
        /// </summary>
        /// <param name="modelName">The name of the model for which to retrieve the settings. Cannot be null.</param>
        /// <returns>The settings value associated with the specified model name, or <see langword="null"/> if the model name
        /// does not exist in the settings data.</returns>
        public bool? GetSettings(string modelName)
        {
            if (!_settingsData.Projects.ContainsKey(modelName) || modelName == null)
            {
                return null;
            }

            return _settingsData.Projects[modelName];
        }

        /// <summary>
        /// Saves the current application settings to a file in JSON format.
        /// </summary>
        /// <remarks>The settings are serialized with indentation for readability and written to the file
        /// specified by the settings file path. Ensure that the file path is valid and accessible to avoid
        /// exceptions.</remarks>
        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            // Serialize the current _instance
            string jsonString = JsonSerializer.Serialize(_settingsData, options);
            File.WriteAllText(_settingsFilePath, jsonString);
        }

        /// <summary>
        /// Loads the application settings from a file, initializing the settings data.
        /// </summary>
        /// <remarks>If the settings file exists, the method reads and deserializes its content into the
        /// settings data object. If the file does not exist, it initializes a new settings data object and saves it to
        /// create the file.</remarks>
        private void LoadSettings()
        {
            if (File.Exists(_settingsFilePath))
            {
                string jsonString = File.ReadAllText(_settingsFilePath);
                //deserialize into the runtime SettingsData object or create a new settings data if empty
                _settingsData = JsonSerializer.Deserialize<PersistentData>(jsonString) ?? new PersistentData();
            }
            else // if this is the first time the user has ever used in the plugin
            {
                _settingsData = new PersistentData();
                SaveSettings();
            }
        }

        /// <summary>
        /// Retrieves the file path for the settings file used by the application.
        /// </summary>
        /// <remarks>This method constructs the path to the settings file located in the user's local
        /// application data folder. If the settings directory does not exist, it attempts to create it. If directory
        /// creation fails, the method returns <see langword="null"/> and displays an error message to the
        /// user.</remarks>
        /// <returns>The full file path to the settings file named "settings.json" within the "WayFinder" subdirectory of the
        /// user's application data folder. Returns <see langword="null"/> if the directory cannot be created.</returns>
        private string GetSettingsFilePath()
        {
            // Get the user's local app data folder
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Create a subfolder for the plugin
            string settingsFolder = Path.Combine(appDataFolder, "WayFinder");

            try
            {
                Directory.CreateDirectory(settingsFolder); // create the actual settings folder
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", "WayFinder couldn't write settings file to \n" +
                    "C:\\Users\\{your_user_name}\\AppData\\Roaming. Contact your administrator.\n" +
                    "WayFinder will be able to run. But it won't be able to save your settings.");
                return null;
            }
            

            return Path.Combine(settingsFolder, "settings.json");
        }

    }
}
