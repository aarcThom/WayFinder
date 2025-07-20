using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reactive.Subjects;


namespace WayFinder.Settings
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
        private readonly BehaviorSubject<string> _focusedModel = new BehaviorSubject<string>(null); 
        private readonly BehaviorSubject<bool> _focusedActiveState = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> _focusedDebugState = new BehaviorSubject<bool>(false);


        // ========================================= PROPERTIES ==================================================================
        // provides the global point of access for the single _instance



        public static ModelSettings Instance => _instance;

        // Expose the subjects as an IObservable for subscribers.
        // This prevents subscribers from pushing values into the subject.
        public IObservable<string> FocusedModel => _focusedModel;
        public IObservable<bool> FocusedActiveState => _focusedActiveState;
        public IObservable<bool> FocusedDebugState => _focusedDebugState;

        // In case I need the active model elsewhere
        public String ActiveModel => _focusedModel.Value;

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
            _focusedModel.OnNext(modelName);
            _focusedActiveState.OnNext(activeState);
            _focusedDebugState.OnNext(debugState);

            // add it to the dictionary
            _openModelsActiveStatuses[_focusedModel.Value] = activeState;
            _openModelsDebugStatuses[_focusedModel.Value] = debugState;
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

            _focusedModel.OnNext(modelName);
            _focusedDebugState.OnNext(_openModelsDebugStatuses[modelName]);
            _focusedActiveState.OnNext(_openModelsActiveStatuses[modelName]);

        }


        /// <summary>
        /// Disposes of the specified model by removing its active and debug statuses.
        /// </summary>
        /// <remarks>This method removes the specified model from the active and debug status collections,
        /// and sets the focused model to null. Ensure that <paramref name="modelName"/> is valid  and exists in the
        /// collections before calling this method.</remarks>
        /// <param name="modelName">The name of the model to be disposed. Must not be null or empty.</param>
        public void DisposeModel(string modelName)
        {
            if (_openModelsActiveStatuses.ContainsKey(modelName))
            {
                _openModelsActiveStatuses.Remove(modelName);
            }

            if (_openModelsDebugStatuses.ContainsKey(modelName))
            {
                _openModelsDebugStatuses.Remove(modelName);
            }

            _focusedModel.OnNext(null);
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
            if (_focusedModel.Value == null)
            {
                TaskDialog.Show("Error", "Current Model not set.");
                return false;
            }

            if (!_openModelsDebugStatuses.ContainsKey(_focusedModel.Value))
            {
                TaskDialog.Show("Error", "Current Model doesn't have debug state set.");
                return false;
            }
            return _focusedDebugState.Value;
        }


        /// <summary>
        /// Sets the debug state for the currently focused model.
        /// </summary>
        /// <remarks>If no model is currently focused, an error dialog is displayed and the operation is
        /// aborted.</remarks>
        /// <param name="debugState">A boolean value indicating the desired debug state.  <see langword="true"/> to enable debugging; otherwise,
        /// <see langword="false"/>.</param>
        public void SetModelDebugState(bool debugState)
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current model not set.");
                return;
            }

            _focusedDebugState.OnNext(debugState);
            _openModelsDebugStatuses[_focusedModel.Value] = debugState;
        }


        /// <summary>
        /// Determines whether the currently focused model is in an active state.
        /// </summary>
        /// <remarks>Displays an error dialog if the focused model is not set or if its active state is
        /// not defined.</remarks>
        /// <returns><see langword="true"/> if the focused model is active; otherwise, <see langword="false"/>.</returns>
        public bool GetModelActiveState()
        {
            if (_focusedModel == null)
            {
                TaskDialog.Show("Error", "Current Model not set.");
                return false;
            }

            if (!_openModelsActiveStatuses.ContainsKey(_focusedModel.Value))
            {
                TaskDialog.Show("Error", "Current Model doesn't have active state set.");
                return false;
            }
            return _focusedActiveState.Value;
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

            _focusedDebugState.OnNext(activeState);
            _openModelsActiveStatuses[_focusedModel.Value] = activeState;
        }
    }
}
