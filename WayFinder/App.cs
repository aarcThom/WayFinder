using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using WayFinder.Settings;
using WayFinder.RevitObjects;

namespace WayFinder
{
    public class App : IExternalApplication
    {

        // It's most reliable to store the UIapplication object from OnStartup as a class member
        // Instead of creating a new instance each OnDocumentChanged
        private static UIControlledApplication _controlledUIApp;

        private static readonly BehaviorSubject<string> _focusedModel = new BehaviorSubject<string>(null); // the current model
        public static IObservable<string> FocusedModel => _focusedModel; // allows behaviour to be subscribed to


        public Result OnStartup(UIControlledApplication application)
        {
            _controlledUIApp = application; //store the UI App as a class member

            string assPath = Assembly.GetExecutingAssembly().Location; // Get the location of Revit

            // ===================== INITIALIZE SETTINGS ============================================================================================
            _ = PersistentSettings.Instance;
            _ = AppSettings.Instance;
            _ = WFButtons.Instance;
            // ===================== ADD RIBBON PANEL ===============================================================================================
            WFButtons.Instance.GenerateRibbonPanel(_controlledUIApp, assPath);

            // ===================== SUBSCRIBE TO EVENTS ============================================================================================
            try
            {
                // Subscribe to Events
                _controlledUIApp.ControlledApplication.DocumentOpened += OnDocumentOpened;
                _controlledUIApp.ControlledApplication.DocumentClosing += OnDocumentClosing;
                _controlledUIApp.ControlledApplication.DocumentClosed += OnDocumentClosed;
                _controlledUIApp.ViewActivated += OnViewActivated;
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

        }

        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            // Handle the view activated event
            Document doc = e.Document;
            _focusedModel.OnNext(doc.Title);

            AppSettings.Instance.test();
        }

        /// <summary>
        /// This method is called just BEFORE a document closes.
        /// It is cancellable. We use it to get and store the document's info.
        /// </summary>
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            //grab the currently open model just in case user closes without switching tabs
            string prevFocus = _focusedModel.Value;

            _focusedModel.OnNext(args.Document.Title); // Set the document to active
            AppSettings.Instance.test(); // replace this with the dispose methods

            // reset the current model if user closed model without switching tabs
            if (prevFocus != _focusedModel.Value)
            {
                _focusedModel.OnNext(prevFocus);
                AppSettings.Instance.test();
            }

        }

        /// <summary>
        /// Handles the event when a document is closed in the application. Happens AFTER doc is closed
        /// </summary>
        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {

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
