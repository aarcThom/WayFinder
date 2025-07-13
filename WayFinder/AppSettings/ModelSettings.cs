using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reactive.Subjects;


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


        private readonly Dictionary<string, bool> _modelActiveStatus = new Dictionary<string, bool>(); // keeping the state of session open models
        private readonly Dictionary<string, bool> _modelDebugStatus = new Dictionary<string, bool>(); // keeping the state of session open models

        private string _focusedModel; // the currently focused model

        private bool _focusedActiveState; // the active state of the currently focused model

        // Use a BehaviorSubject to store and broadcast the last value.
        // It's initialized with the default state (false).
        private readonly BehaviorSubject<bool> _focuedDebugState = new BehaviorSubject<bool>(false);


        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static ModelSettings Instance => _instance;

        // Expose the subject as an IObservable for subscribers.
        // This prevents subscribers from pushing values into the subject.
        public IObservable<bool> FocusedDebugState => _focuedDebugState;

        // The property that controls the state.
        public bool FocusedActive
        {
            get => _focuedDebugState.Value;
            set
            {
                // When the value changes, push a notification to all subscribers.
                _focuedDebugState.OnNext(value);
            }
        }

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
        /// Determines whether the specified model is in debug mode.
        /// </summary>
        /// <remarks>If the specified model name does not exist in the debug status dictionary, the method
        /// returns <see langword="false"/>.</remarks>
        /// <param name="modelName">The name of the model to check for debug status. Cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the model is in debug mode; otherwise, <see langword="false"/>.</returns>
        public bool GetModelDebugState(string modelName)
        {
            if (!_modelDebugStatus.ContainsKey(modelName))
            {
                return false;
            }
            return _modelDebugStatus[modelName];
        }


        /// <summary>
        /// Sets the debug state for a specified model.
        /// </summary>
        /// <remarks>This method updates the internal debug status for the specified model, allowing for
        /// model-specific debugging control.</remarks>
        /// <param name="modelName">The name of the model for which to set the debug state. Cannot be null or empty.</param>
        /// <param name="debugState">The debug state to set for the model. <see langword="true"/> to enable debugging; otherwise, <see
        /// langword="false"/>. Defaults to <see langword="false"/>.</param>
        public void SetModelDebugState(string modelName, bool debugState = false)
        {
            _focusedModel = modelName;
            FocusedActive = debugState;

            _modelDebugStatus[modelName] = debugState;
        }


        /// <summary>
        /// Determines whether the specified model is in an active state.
        /// </summary>
        /// <remarks>If the specified model name does not exist in the current context, the method returns
        /// <see langword="false"/>.</remarks>
        /// <param name="modelName">The name of the model to check.</param>
        /// <returns><see langword="true"/> if the model is in an active state; otherwise, <see langword="false"/>.</returns>
        public bool GetModelState(string modelName)
        {

            if (!_modelActiveStatus.ContainsKey(modelName))
            {
                return false;
            }
            return _modelActiveStatus[modelName];
        }

        /// <summary>
        /// Sets the specified model as the focused model and updates its active state.
        /// </summary>
        /// <remarks>This method updates the internal state to reflect the specified model as the focused
        /// model and adjusts its active status accordingly.</remarks>
        /// <param name="modelName">The name of the model to be set as focused. Cannot be null or empty.</param>
        /// <param name="activeState">A boolean value indicating whether the model is active. <see langword="true"/> if the model is active;
        /// otherwise, <see langword="false"/>.</param>
        public void SetModelState(string modelName, bool activeState)
        {
            _focusedModel = modelName;
            _focusedActiveState = activeState;

            _modelActiveStatus[modelName] = activeState;
        }
    }
}
