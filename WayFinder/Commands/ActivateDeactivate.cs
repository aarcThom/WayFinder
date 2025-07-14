using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WayFinder.AppSettings;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace WayFinder.Commands
{
    [Transaction(TransactionMode.Manual)]

    [Regeneration(RegenerationOption.Manual)]
    public class ActivateDeactivate : WFCommand
    {
        protected override Result ExecuteCommand(UIApplication uiApp, UIDocument uiDoc, Document doc, Application app, ref string message, ElementSet elements)
        {
            string docTitle = doc.Title;

            if (!ModelSettings.Instance.GetModelDebugState(docTitle))
            {
                // If the model is not in debug mode, activate it
                ModelSettings.Instance.SetModelDebugState(docTitle, true);
                ModelSettings.Instance.FocusedActive = true; // Set the focused model to active
            }
            else
            {
                // If the model is already in debug mode, deactivate it
                ModelSettings.Instance.SetModelDebugState(docTitle, false);
                ModelSettings.Instance.FocusedActive = false; // Set the focused model to inactive
            }

            return Result.Succeeded;
        }
    }
}
