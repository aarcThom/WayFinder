using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WayFinder.RevitObjects;


namespace WayFinder.Settings
{

    public sealed class AppSettings
    {

        // ========================================== FIELDS ======================================================================

        // the singleton _instance field 
        // private ensures the field can only be accessed from within the singleton class
        // static means that the field belongs to the class itself, not an _instance, hence there is only one
        // readonly ensures that _instance can only be assigned a value once. After the initial declaration, it cant be changed.
        private static readonly AppSettings _instance = new AppSettings();

        private readonly IDisposable _focusedModelSubscribe; // the subscription to the ModelSettings
        private string _focusedModel; // the currently focused model




        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static AppSettings Instance => _instance;

        // ===================================== CONSTRUCTORS ====================================================================

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefield init
        // this ensures static fields are initialized exactly when the class is first accessed.
        static AppSettings()
        {
        }

        // the _instance constructor for the class
        // private ensure that code outside the class can't use 'new AppSettings'
        private AppSettings()
        {
            // subscribe to the currently focused model
            _focusedModelSubscribe = App.FocusedModel.Subscribe(currentModel =>
            {
                _focusedModel = currentModel;
            });
        }

        public void test()
        {
            TaskDialog.Show("cool", $"The current model is {_focusedModel}");
        }
    }
}
