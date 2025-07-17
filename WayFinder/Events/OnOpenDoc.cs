using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using WayFinder.AppSettings;

namespace WayFinder.Events
{
    public static class OnOpenDoc
    {
        public static void InitializeModel(string modelName)
        {
            // there was an error in activating the app
            if (!PersistentSettings.Instance.AppWorking)
            {
                return;
            }

            // see if the opened model is saved in persistent settings
            bool? docSavedSettings = PersistentSettings.Instance.GetSettings(modelName);

            if (!docSavedSettings.HasValue) // the current model doesn't have an entry in the persistant settings
            {
                // prompt user for settings and set them
                bool updatedSetting = AddModelToSettings(modelName);
                docSavedSettings = updatedSetting;
            }

            // focus the current model
            ModelSettings.Instance.SetCurrentModel(modelName);

            // set the current active state for the model
            ModelSettings.Instance.SetModelActiveState(docSavedSettings.Value);

            // set the active state for the buttons
            WFButtons.Instance.ActivateDeactivateButtons(docSavedSettings.Value);

            //set debug state to false to start
            ModelSettings.Instance.SetModelDebugState();
        }

        private static bool AddModelToSettings(string modelName)
        {
            // prompt the user
            bool defaultActiveState = Helpers.UI.BoolFromYesNoDialog("WayFinder", "Would you like to enable WayFinder for this model? \n" +
                "You may change this setting later with the 'Enable/Disable' button.");

            // write default settings to JSON file
            PersistentSettings.Instance.SetSettings(modelName, defaultActiveState);

            return defaultActiveState;
        }
    }
}
