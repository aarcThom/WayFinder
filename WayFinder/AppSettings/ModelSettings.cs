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


        // keeping track of the currently open models
        private readonly Dictionary<string, bool> _openModelsActiveStatuses = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> _openModelsDebugStatuses = new Dictionary<string, bool>();

        // the currently focused model and its active and debug states
        private string _focusedModel = null; 
        private readonly BehaviorSubject<bool> _focusedActiveState = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> _focusedDebugState = new BehaviorSubject<bool>(false);


        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance
        public static ModelSettings Instance => _instance;


        public string CurrentModel { get => _focusedModel; }


        // Expose the subject as an IObservable for subscribers.
        // This prevents subscribers from pushing values into the subject.
        public IObservable<bool> FocusedActiveState => _focusedActiveState;

        // The property that controls the active state.
        public bool FocusedActive
        {
            get => _focusedActiveState.Value;
            set
            {
                // When the value changes, push a notification to all subscribers.
                _focusedActiveState.OnNext(value);
            }
        }



        // Expose the subject as an IObservable for subscribers.
        // This prevents subscribers from pushing values into the subject.
        public IObservable<bool> FocusedDebugState => _focusedDebugState;

        // The property that controls the debug state.
        public bool FocusedDebug
        {
            get => _focusedDebugState.Value;
            set
            {
                // When the value changes, push a notification to all subscribers.
                _focusedDebugState.OnNext(value);
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
        /// Initializes the model with the specified name and sets its active and debug states.
        /// </summary>
        /// <remarks>This method sets the specified model as the current focused model and updates its
        /// active and debug states in the internal tracking dictionaries.</remarks>
        /// <param name="modelName">The name of the model to initialize. Cannot be null or empty.</param>
        /// <param name="activeState">A <see langword="true"/> to set the model as active; otherwise, <see langword="false"/>.</param>
        /// <param name="debugState">A <see langword="true"/> to enable debug mode for the model; otherwise, <see langword="false"/>.</param>
        public void InitializeModel(string modelName, bool activeState, bool debugState)
        {
            // set the new model to current
            _focusedModel = modelName;
            FocusedActive = activeState;
            FocusedDebug  = debugState;

            // add it to the dictionary
            _openModelsActiveStatuses[_focusedModel] = activeState;
            _openModelsDebugStatuses[_focusedModel] = debugState;
        }

        /// <summary>
        /// Sets the current model to the specified model name.
        /// </summary>
        /// <remarks>This method updates the focused model and its associated debug and active statuses. 
        /// If the specified model name is not registered, an error dialog is displayed and the operation is
        /// aborted.</remarks>
        /// <param name="modelName">The name of the model to set as the current model. Must be a registered model name.</param>
        public void SetCurrentModel(string modelName)
        {
            if (!_openModelsActiveStatuses.ContainsKey(modelName) ||
                !_openModelsDebugStatuses.ContainsKey(modelName))
                {
                TaskDialog.Show("Error", "The open model is not registered by Way Finder.");
                return;
                }

            _focusedModel = modelName;
            FocusedDebug = _openModelsDebugStatuses[modelName];
            FocusedActive = _openModelsActiveStatuses[modelName];

        }
        
        
        /// <summary>
        /// Determines whether the current model has a debug state set and retrieves its status.
        /// </summary>
        /// <remarks>Displays an error message if the current model is not set or if the debug state is
        /// not available.</remarks>
        /// <returns><see langword="true"/> if the current model has a debug state set and it is active; otherwise, <see
        /// langword="false"/>.</returns>
        public bool GetModelDebugState()
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current Model not set.");
                return false;
            }

            if (!_openModelsDebugStatuses.ContainsKey(_focusedModel))
            {
                TaskDialog.Show("Error", "Current Model doesn't have debug state set.");
                return false;
            }
            return _openModelsDebugStatuses[_focusedModel];
        }


        /// <summary>
        /// Sets the debug state for the current model.
        /// </summary>
        /// <remarks>If the current model is not set, an error dialog is displayed and the operation is
        /// aborted.</remarks>
        /// <param name="debugState">A boolean value indicating the desired debug state.  <see langword="true"/> to enable debugging; otherwise,
        /// <see langword="false"/>.</param>
        public void SetModelDebugState(bool debugState = false)
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current model not set.");
                return;
            }

            FocusedDebug = debugState;

            _openModelsDebugStatuses[_focusedModel] = debugState;
        }


        /// <summary>
        /// Determines whether the currently focused model is in an active state.
        /// </summary>
        /// <remarks>Displays an error dialog if the focused model is not set or if its active state is
        /// not defined.</remarks>
        /// <returns><see langword="true"/> if the focused model is active; otherwise, <see langword="false"/>.</returns>
        public bool GetModelState()
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current Model not set.");
                return false;
            }

            if (!_openModelsActiveStatuses.ContainsKey(_focusedModel))
            {
                TaskDialog.Show("Error", "Current Model doesn't have active state set.");
                return false;
            }
            return _openModelsActiveStatuses[_focusedModel];
        }

        /// <summary>
        /// Sets the active state of the currently focused model.
        /// </summary>
        /// <remarks>If no model is currently focused, an error dialog is displayed and the operation is
        /// aborted.</remarks>
        /// <param name="activeState">A boolean value indicating the desired active state.  <see langword="true"/> to activate the model;
        /// otherwise, <see langword="false"/>.</param>
        public void SetModelActiveState(bool activeState)
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current model not set.");
                return;
            }

            _focusedActiveState = activeState;

            _openModelsActiveStatuses[_focusedModel] = activeState;
        }
    }
}
