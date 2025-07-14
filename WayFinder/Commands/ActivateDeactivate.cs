using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace WayFinder.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public abstract class ActivateDeactivate : IExternalCommand
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
                return Result.Succeeded; // This is a placeholder. Replace with your command logic.
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
    }
}
