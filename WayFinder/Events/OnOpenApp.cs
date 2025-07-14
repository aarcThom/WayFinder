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
    public static class OnOpenApp
    {

        private static List<Dictionary<string, string>> _buttonSettings = new List<Dictionary<string, string>>()
        {
            // UI NAME, UI DESCRIPTION, CLASSNAME, TOOLTIP, ICONPATH ==============================================

            // ACTIVATE / DEACTIVATE
            new Dictionary<string, string>()
            {
                {"uiName" , "Enable / Disable"},
                {"uiText" , "Enable or Disable WayFarer"},
                {"className" , "ActivateDeactivate"},
                {"toolTip", "Enable or Disable WayFarer"},
                {"iconPath", "WayFinder.Resources.Icons.updateSign_32.png" }
            }
        };

        public static void RibbonPanel(UIControlledApplication application, string assemblyPath)
        {
            
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("WayFinder"); // create the ribbon panel

            // Add to the ribbon panel and populate with the proper data
            List<PushButtonData> buttonData = new List<PushButtonData>();
            foreach(var butInfo in _buttonSettings)
            {
                buttonData.Add(new PushButtonData(butInfo["uiName"], butInfo["uiText"], assemblyPath, $"WayFinder.Commands.{butInfo["className"]}"));
            }

            List<PushButton> buttonList = new List<PushButton>();
            foreach (var butData in buttonData)
            {
                buttonList.Add(ribbonPanel.AddItem(butData) as PushButton);
            }

            int counter = 0;
            foreach (var button in buttonList)
            {
                var tooltip = _buttonSettings[counter]["toolTip"];
                var image = Resources.ResourceUtils.GetEmbeddedImage(_buttonSettings[counter]["iconPath"]);

                button.Image = image;
                button.ToolTip = tooltip;

                counter++;
            }

        }
    }
}
