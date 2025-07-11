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

namespace WayFinder
{
    public class App : IExternalApplication
    {

        // It's most reliable to store the UIapplication object from OnStartup as a class member
        // Instead of creating a new instance each OnDocumentChanged
        private static UIControlledApplication _controlledUIApp;


        public Result OnStartup(UIControlledApplication application)
        {
            _controlledUIApp = application;

            // ===================== SUBSCRIBE TO EVENTS ============================================================================================

            try
            {
                // Subscribe to the DocumentOpened Event
                _controlledUIApp.ControlledApplication.DocumentOpened += OnDocumentOpened;
            }
            catch (System.Exception)
            {
                return Result.Failed;
            }

            // ===================== RETURN ==========================================================================================================

            return Result.Succeeded;
        }


        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            return;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // ====================== UNSUBSCRIBE FROM EVENTS ========================================================================================
            _controlledUIApp.ControlledApplication.DocumentOpened -= OnDocumentOpened;

            // ====================== RETURN ==========================================================================================================
            return Result.Succeeded;
        }

    }
}
