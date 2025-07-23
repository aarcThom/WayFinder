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

        private static readonly BehaviorSubject<string> _currentModelName = new BehaviorSubject<string>(null); // the current model
        public static IObservable<string> CurrentModel => _currentModelName; // allows behaviour to be subscribed to


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
            SetCurrentModel(model:doc);

            AppSettings.Instance.test();
        }

        /// <summary>
        /// This method is called just BEFORE a document closes.
        /// It is cancellable. We use it to get and store the document's info.
        /// </summary>
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            // remove the model from the app settings
            AppSettings.Instance.RemoveModel(args.Document.Title);

            AppSettings.Instance.test();

        }

        /// <summary>
        /// Handles the event when a document is closed in the application. Happens AFTER doc is closed
        /// </summary>
        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {

        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // 
            // ====================== UNSUBSCRIBE FROM EVENTS ========================================================================================
            _controlledUIApp.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            _controlledUIApp.ControlledApplication.DocumentClosing -= OnDocumentClosing;
            _controlledUIApp.ControlledApplication.DocumentClosed -= OnDocumentClosed;

            // ====================== RETURN ==========================================================================================================
            return Result.Succeeded;
        }


        // ================================== HELPER FUNCTIONS ========================================================================================

        /// <summary>
        /// Sets the current model by updating the focused model's title.
        /// </summary>
        /// <remarks>If both <paramref name="modelName"/> and <paramref name="model"/> are provided, the
        /// method prioritizes the <paramref name="model"/> parameter.</remarks>
        /// <param name="modelName">The name of the model to set as the current model. If <paramref name="model"/> is provided, this parameter
        /// is ignored.</param>
        /// <param name="model">The <see cref="Document"/> object representing the model to set as the current model. If the model is a
        /// family document, the operation is aborted.</param>
        private void SetCurrentModel(string modelName = null, Document model = null)
        {
            if (model != null && model.IsFamilyDocument)
            {
                return;
            }

            if (model != null)
            {
                _currentModelName.OnNext(model.Title);
                return;
            }

            if (modelName != null)
            {
                _currentModelName.OnNext(modelName);
            }
            
        }
    }
}
