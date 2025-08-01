using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WayFinder.RevitObjects;


namespace WayFinder.Settings
{
    /// <summary>
    /// Provides application-wide settings and manages the state of models within the application.
    /// </summary>
    /// <remarks>The <see cref="AppSettings"/> class implements the singleton pattern to ensure a single
    /// instance is used throughout the application. It manages the settings for models, including their active status,
    /// and provides access to the currently focused model. The class also handles subscription to model changes and
    /// updates the application state accordingly.</remarks>
    public sealed class AppSettings
    {

        // ========================================== FIELDS ======================================================================

        // the singleton _instance field 
        // private ensures the field can only be accessed from within the singleton class
        // static means that the field belongs to the class itself, not an _instance, hence there is only one
        // readonly ensures that _instance can only be assigned a value once. After the initial declaration, it cant be changed.
        private static readonly AppSettings _instance = new AppSettings();

        private readonly IDisposable _currentModelSubscribe; // the subscription to the ModelSettings
        private string _currentModelName; // the currently focused model

        // the dictionary that holds all the model settings
        private Dictionary<string, CurrentModel> _openModels = new Dictionary<string, CurrentModel>();

        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static AppSettings Instance => _instance;

        // retrieves the model settings of the currently focused model
        public CurrentModel CurrentModel => _openModels[_currentModelName];

        // ===================================== CONSTRUCTORS ====================================================================

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefield init
        // this ensures static fields are initialized exactly when the class is first accessed.
        static AppSettings()
        {
        }

        // the _instance constructor for the class
        // private ensure that code outside the class can't use 'new AppSettings'
        private AppSettings()
        {
            // subscribe to the currently focused model
            _currentModelSubscribe = App.CurrentModel.Subscribe(currentModel =>
            {

                // this will happen the first time the instance is initialized
                // before a document is opened
                if (currentModel == null) 
                { 
                    return; 
                }

                _currentModelName = currentModel;

                // if it is a newly opened document
                if (_currentModelName != null && !_openModels.ContainsKey(_currentModelName))
                {
                    // check if it's in persistent settings and get active status
                    bool activeStatus = CheckPersistentSettings(_currentModelName);

                    // add the current model to the dictionary
                    _openModels[_currentModelName] = new CurrentModel(activeStatus);
                }

                // activate or deactivate the ribbon panel
                WFButtons.Instance.ActivateDeactivateButtons(CurrentModel.IsActive);
            });
        }

        // ===================================== METHODS ===========================================================================
        
        /// <summary>
        /// Toggles the debug status of the currently active model. 
        /// </summary>
        /// <remarks>This method switches the debug status of the model identified by the current model
        /// name. The specific behavior of toggling the debug status is determined by the model
        /// implementation.</remarks>
        public void ToggleModelDebugStatus()
        {
            // toggle the debug status of the current model
            _openModels[_currentModelName].ToggleDebugStatus();
        }

        /// <summary>
        /// Toggles the active setting of the current model.
        /// </summary>
        /// <remarks>This method changes the active state of the current model and updates the UI
        /// accordingly.</remarks>
        public void ToggleModelActiveStatus()
        {
            // toggle the active setting of current model
            _openModels[_currentModelName].ToggleActiveSettings();

            // activate or deactivate the ribbon panel
            WFButtons.Instance.ActivateDeactivateButtons(CurrentModel.IsActive);
        }


        /// <summary>
        /// Removes the specified model from the collection of open models.
        /// </summary>
        /// <remarks>If the specified model is currently active, its status is updated in the persistent
        /// settings. Additionally, if the model being removed is the current model, the current model reference is
        /// cleared.</remarks>
        /// <param name="modelToRemove">The name of the model to be removed. Must not be null or empty.</param>
        public void RemoveModel(string modelToRemove)
        {
            
            if (_openModels.ContainsKey(modelToRemove))
            {
                // update the persistant settings
                bool modelStatus = _openModels[modelToRemove].IsActive;
                PersistentSettings.Instance.SetSettings(modelToRemove, modelStatus);

                // clear the current model if it's being closed
                if (modelToRemove == _currentModelName)
                {
                    _currentModelName = null;
                }

                // remove it from the AppSettings dictionary
                // WILL NEED TO ENSURE THINGS ARE PROPERLY DISPOSED. +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++!!!!!!!!!!!!!!!!!!!!
                _openModels.Remove(modelToRemove);
            }
        }

        /// <summary>
        /// Releases the resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method should be called when the instance is no longer needed to free up
        /// resources. It is safe to call this method multiple times.</remarks>
        public void Dispose()
        {
            if (_currentModelSubscribe != null)
            {
                _currentModelSubscribe.Dispose();
            }
        }

        /// <summary>
        /// Checks if the specified model is configured in the persistent settings.
        /// </summary>
        /// <remarks>If the model is not found in the persistent settings, the method prompts the user to
        /// add it.</remarks>
        /// <param name="modelName">The name of the model to check in the settings.</param>
        /// <returns>true if the model is set to active in the settings;  false if the application is not working or the
        /// model is set to not-active in the settings. </returns>
        private static bool CheckPersistentSettings(string modelName)
        {
            // there was an error in activating the app
            if (!PersistentSettings.Instance.AppWorking)
            {
                TaskDialog.Show("Error", "WayFinder is not working. Couldn't add document to settings.");
                return false;
            }

            // see if the opened model is saved in persistent settings
            bool? docSavedSettings = PersistentSettings.Instance.GetSettings(modelName);

            if (!docSavedSettings.HasValue) // the current model doesn't have an entry in the persistant settings
            {
                // prompt user for settings and set them
                return AddModelToSettings(modelName);
            }

            return docSavedSettings.Value;
        }

        /// <summary>
        /// Adds a model to the settings with a user-defined active state.
        /// </summary>
        /// <remarks>This method prompts the user to decide whether the model should be enabled by
        /// default.  The user's choice is saved in the persistent settings and can be modified later.</remarks>
        /// <param name="modelName">The name of the model to be added to the settings. Cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the model is set to be active by default; otherwise, <see langword="false"/>. </returns>
        private static bool AddModelToSettings(string modelName)
        {
            // prompt the user
            bool defaultActiveState = Helpers.UI.BoolFromYesNoDialog("WayFinder", "Would you like to enable WayFinder for this model? \n" +
                "You may change this setting later with the 'Enable/Disable' button.");

            // write default settings to JSON file
            PersistentSettings.Instance.SetSettings(modelName, defaultActiveState);

            return defaultActiveState;
        }

        public void test()
        {
            string test_text = string.Empty;

            foreach (var item in _openModels.Keys)
            {
                test_text += $"{item}, ";
            }

            TaskDialog.Show("cool", $"{_currentModelName} is {CurrentModel.DebugStatus}. \n The open models are {test_text}.");
        }
    }
}
