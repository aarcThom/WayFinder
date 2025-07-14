using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace WayFinder.Commands
{
   public abstract class WFCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the Revit Application
            UIApplication uiApp = commandData.Application;

            // Get the database of the active document
            Document doc = uiApp.ActiveUIDocument.Document;

            // Get the active UI document
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            // Get the active application
            Application app = doc.Application;

            Result result;

            try
            {
                // Implement the specific command logic in the derived class
                return ExecuteCommand(uiApp, uiDoc, doc, app, ref message, elements);
            }

            // This is the specific block that catches the user pressing Esc
            catch (OperationCanceledException)
            {
                // It's good practice to return "Cancelled" to Revit in this case.
                result = Result.Cancelled;
            }
            // A general catch block for any other unexpected errors
            catch (Exception ex)
            {
                var warningDialog = new TaskDialog("Error");
                warningDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning; // Sets the yellow warning icon
                warningDialog.MainInstruction = "Error:";
                warningDialog.MainContent = ex.Message;
                warningDialog.CommonButtons = TaskDialogCommonButtons.Close;

                warningDialog.Show();

                result = Result.Failed;
            }

            return result;
        }

        /// <summary>
        /// This method must be implemented by any class that inherits from CommandBase.
        /// It contains the specific logic for the command.
        /// </summary>
        protected abstract Result ExecuteCommand(UIApplication uiApp, UIDocument uiDoc, Document doc, Application app, ref string message, ElementSet elements);
    }
}
