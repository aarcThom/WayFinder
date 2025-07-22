using WayFinder.Settings;

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

            // Initialize the current model with saved or prompted active state and debug = False
            //ModelSettings.Instance.InitializeModel(modelName, docSavedSettings.Value, false);

            // set the active state for the buttons
            WFButtons.Instance.ActivateDeactivateButtons(docSavedSettings.Value);
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
