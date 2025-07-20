using Autodesk.Revit.UI;
using System.Collections.Generic;


namespace WayFinder.Settings
{

    public sealed class WFButtons
    {

        // ========================================== FIELDS ======================================================================

        // the singleton _instance field 
        // private ensures the field can only be accessed from within the singleton class
        // static means that the field belongs to the class itself, not an _instance, hence there is only one
        // readonly ensures that _instance can only be assigned a value once. After the initial declaration, it cant be changed.
        private static readonly WFButtons _instance = new WFButtons();

        private List<PushButton> _buttons; // list of buttons used for activating / deactivating

        private static string _activateButtonName = "Enable / Disable"; // We note this name as we DO NOT want to ever deactivate it

        // The list of buttons settings used to populate the ribbon bar
        private static List<Dictionary<string, string>> _buttonSettings = new List<Dictionary<string, string>>()
        {
            // UI NAME, UI DESCRIPTION, CLASSNAME, TOOLTIP, ICONPATH ==============================================

            // ACTIVATE / DEACTIVATE
            new Dictionary<string, string>()
            {
                {"uiName" , _activateButtonName},
                {"uiText" , "Enable or Disable WayFarer"},
                {"className" , "ActivateDeactivate"},
                {"toolTip", "Enable or Disable WayFarer"},
                {"iconPath", "WayFinder.Resources.Icons.updateSign_32.png" }
            },
            // Manually update the sign info
            new Dictionary<string, string>()
            {
                {"uiName" , "Update Info"},
                {"uiText" , "Update Info for Sign(s)"},
                {"className" , "UpdateSignInfo"},
                {"toolTip", "Update sign(s) info"},
                {"iconPath", "WayFinder.Resources.Icons.updateSign_32.png" }
            }
        };


        // ========================================= PROPERTIES ==================================================================
        public static WFButtons Instance => _instance; // provides the global point of access for the single _instance

        // ===================================== CONSTRUCTORS ====================================================================

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefield init
        // this ensures static fields are initialized exactly when the class is first accessed.
        static WFButtons()
        {
        }

        // the _instance constructor for the class
        // private ensure that code outside the class can't use 'new AppSettings'
        private WFButtons()
        {
        }

        // =====================================METHODS===========================================================================
        
        /// <summary>
        /// Generates a ribbon panel in the Revit user interface and populates it with buttons based on predefined
        /// settings.
        /// </summary>
        /// <remarks>This method creates a ribbon panel named "WayFinder" and adds buttons to it. Each
        /// button is configured with a name, text, tooltip, and icon as specified in the button settings. The buttons
        /// are linked to command classes located in the specified assembly.</remarks>
        /// <param name="application">The UIControlledApplication instance used to create the ribbon panel.</param>
        /// <param name="assemblyPath">The file path to the assembly containing the command classes for the buttons.</param>
        public void GenerateRibbonPanel(UIControlledApplication application, string assemblyPath)
        {

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("WayFinder"); // create the ribbon panel

            // Add to the ribbon panel and populate with the proper data
            List<PushButtonData> buttonData = new List<PushButtonData>();
            foreach (var butInfo in _buttonSettings)
            {
                buttonData.Add(new PushButtonData(butInfo["uiName"], butInfo["uiText"], assemblyPath, $"WayFinder.Commands.{butInfo["className"]}"));
            }

            _buttons = new List<PushButton>();
            foreach (var butData in buttonData)
            {
                _buttons.Add(ribbonPanel.AddItem(butData) as PushButton);
            }

            int counter = 0;
            foreach (var button in _buttons)
            {
                var tooltip = _buttonSettings[counter]["toolTip"];
                var image = Resources.ResourceUtils.GetEmbeddedImage(_buttonSettings[counter]["iconPath"]);

                button.Image = image;
                button.ToolTip = tooltip;

                counter++;
            }

        }

        /// <summary>
        /// Activates or deactivates a set of buttons based on the specified state.
        /// </summary>
        /// <remarks>The button with the name specified by <c>_activateButtonName</c> is not affected by
        /// this method.</remarks>
        /// <param name="activeState">A <see langword="bool"/> value indicating whether the buttons should be activated  (<see langword="true"/>)
        /// or deactivated (<see langword="false"/>).</param>
        public void ActivateDeactivateButtons(bool activeState)
        {
            foreach (var button in _buttons)
            {
                if (button.Name != _activateButtonName)
                {
                    button.Enabled = activeState;
                }
            }
        }
    }
}
