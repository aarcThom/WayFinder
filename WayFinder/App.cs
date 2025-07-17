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
using WayFinder.RevitObjects;

namespace WayFinder
{
    public class App : IExternalApplication
    {

        // It's most reliable to store the UIapplication object from OnStartup as a class member
        // Instead of creating a new instance each OnDocumentChanged
        private static UIControlledApplication _controlledUIApp;

        private static string _docTitle; // The name of the file that was shut down

        private static List<SignCollection> _signCollections = new List<SignCollection>(); // contains all the sign collections for all open documents


        public Result OnStartup(UIControlledApplication application)
        {
            _controlledUIApp = application; //store the UI App as a class member

            string assPath = Assembly.GetExecutingAssembly().Location; // Get the location of Revit

            // ===================== INITIALIZE SETTINGS ============================================================================================
            _ = PersistentSettings.Instance;
            _ = ModelSettings.Instance;
            _ = WFButtons.Instance;
            

            // ===================== ADD RIBBON PANEL ===============================================================================================
            WFButtons.Instance.GenerateRibbonPanel(_controlledUIApp, assPath);

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
            Document doc = e.Document; // access the document
            string docTitle = doc.Title;

            // set the model settings from persistent settings, prompt user for persistent settings if none.
            Events.OnOpenDoc.InitializeModel(docTitle);


            //testing the wall class
            var signs = new SignCollection(ModelSettings.Instance.);
            signs.AddWallSign(docTitle);
            _signCollections.Add(signs);
        }

        /// <summary>
        /// This method is called just BEFORE a document closes.
        /// It is cancellable. We use it to get and store the document's info.
        /// </summary>
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            // The Document object is still accessible here
            _docTitle = args.Document.Title;

            // Set the document to active
            ModelSettings.Instance.SetCurrentModel(_docTitle);
        }

        /// <summary>
        /// Handles the event when a document is closed in the application. Happens AFTER doc is closed
        /// </summary>
        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            // dispose of all the signs / sign collection bound to documents
            foreach (var signColl in _signCollections)
            {
                if (signColl.ModelName == _docTitle)
                {
                    signColl.Dispose();
                }
            }

            // remove the objects from the signCollections list
            _signCollections = _signCollections.Where(sc => sc.ModelName != _docTitle).ToList();

            // Check if a document was actually closed (not just the Revit application itself)
            if (args.DocumentId > -1)
            {
                // write out the last active setting to the persistent settings
                bool modelState = ModelSettings.Instance.GetModelState();
                PersistentSettings.Instance.SetSettings(_docTitle, modelState);
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
