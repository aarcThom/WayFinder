﻿using Autodesk.Revit.ApplicationServices;
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
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace WayFinder.Commands
{
    [Transaction(TransactionMode.Manual)]

    [Regeneration(RegenerationOption.Manual)]
    public class UpdateSignInfo : WFCommand
    {
        protected override Result ExecuteCommand(UIApplication uiApp, UIDocument uiDoc, Document doc, Application app, ref string message, ElementSet elements)
        {
            // INHERITING CLASS CODE GOES HERE /////////
            ////////////////////////////////////////////
            ///////////////////////////////////////////
            return Result.Succeeded;
        }
    }
}
