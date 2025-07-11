using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WayFinder.AppSettings
{

    public sealed class ModelSettings
    {

        // ========================================== FIELDS ======================================================================

        // the singleton _instance field 
        // private ensures the field can only be accessed from within the singleton class
        // static means that the field belongs to the class itself, not an _instance, hence there is only one
        // readonly ensures that _instance can only be assigned a value once. After the initial declaration, it cant be changed.
        private static readonly ModelSettings _instance = new ModelSettings();


        private string _docName;


        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static ModelSettings Instance => _instance;

        // ===================================== CONSTRUCTORS ====================================================================

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefield init
        // this ensures static fields are initialized exactly when the class is first accessed.
        static ModelSettings()
        {
        }

        // the _instance constructor for the class
        // private ensure that code outside the class can't use 'new AppSettings'
        private ModelSettings()
        {
        }

        // =====================================METHODS===========================================================================
        
        /// <summary>
        /// Sets the model name to be used for documentation purposes.
        /// </summary>
        /// <remarks>This method assigns the specified model name to the internal documentation name.
        /// Ensure that <paramref name="modelName"/> is a valid, non-empty string before calling this method.</remarks>
        /// <param name="modelName">The name of the model to set. Cannot be null or empty.</param>
        public void GetModelInfo(string modelName)
        {
            _docName = modelName;
        }
    }
}
