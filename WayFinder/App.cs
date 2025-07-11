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

namespace WayFinder
{
    public class App : IExternalApplication
    {

        // It's most reliable to store the UIapplication object from OnStartup as a class member
        // Instead of creating a new instance each OnDocumentChanged
        private static UIControlledApplication _controlledUIApp;

        private static string _docTitle; // The name of the file that was shut down


        public Result OnStartup(UIControlledApplication application)
        {
            _controlledUIApp = application; //store the UI App as a class member


            // ===================== INITIALIZE SETTINGS ============================================================================================
            _ = PersistentSettings.Instance;


            // ===================== SUBSCRIBE TO EVENTS ============================================================================================
            try
            {
                // Subscribe to the DocumentOpened Event
                _controlledUIApp.ControlledApplication.DocumentOpened += OnDocumentOpened;
                _controlledUIApp.ControlledApplication.DocumentClosing += OnDocumentClosing;
                _controlledUIApp.ControlledApplication.DocumentClosed += OnDocumentClosed;
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

        /// <summary>
        /// This method is called just BEFORE a document closes.
        /// It is cancellable. We use it to get and store the document's info.
        /// </summary>
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            // The Document object is still accessible here
            _docTitle = args.Document.Title;
        }


        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            // Check if a document was actually closed (not just the Revit application itself)
            if (args.DocumentId > -1)
            {
                TaskDialog.Show("Document Closed", $"The document '{_docTitle}' has been closed. This action could not be cancelled.");
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // ====================== UNSUBSCRIBE FROM EVENTS ========================================================================================
            _controlledUIApp.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            _controlledUIApp.ControlledApplication.DocumentClosing -= OnDocumentClosing;
            _controlledUIApp.ControlledApplication.DocumentClosed -= OnDocumentClosed;

            // ====================== RETURN ==========================================================================================================
            return Result.Succeeded;
        }

    }
}
