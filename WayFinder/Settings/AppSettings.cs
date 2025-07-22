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
        private string _currentModel; // the currently focused model

        // the dictionary that holds all the model settings
        private Dictionary<string, CurrentModel> _openModels = new Dictionary<string, CurrentModel>();

        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static AppSettings Instance => _instance;

        // retrieves the model settings of the currently focused model
        public CurrentModel CurrentModel => _openModels[_currentModel];

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
                _currentModel = currentModel;

                if (_currentModel != null && !_openModels.ContainsKey(_currentModel))
                {
                    _openModels[_currentModel] = new CurrentModel();
                }
            });
        }

        // ===================================== METHODS ===========================================================================
        
        /// <summary>
        /// Removes the model with the specified name from the collection.
        /// </summary>
        /// <remarks>If the model with the specified name exists in the collection, it is removed.  Ensure
        /// that any resources associated with the model are properly disposed of before calling this method.</remarks>
        /// <param name="modelName">The name of the model to remove. Must not be null or empty.</param>
        public void RemoveModel(string modelName)
        {
            if (_openModels.ContainsKey(modelName))
            {
                // WILL NEED TO ENSURE THINGS ARE PROPERLY DISPOSED.
                _openModels.Remove(modelName);
            }
        }

        public void test()
        {
            string test = "";
            foreach(string key in _openModels.Keys)
            {
                test += $"{key}, ";
            }

            TaskDialog.Show("cool", $"Models: {test}");
        }
    }
}
