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
            // UI NAME, UI DESCRIPTION, CLASSNAME, TOOLTIP, ICONPATH
        };

        public static void RibbonPanel(UIControlledApplication application)
        {
            string assPath = Assembly.GetExecutingAssembly().Location; // Get the location of Revit

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("WayFinder");


        }
    }
}
