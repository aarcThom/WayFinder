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

    public class CurrentModel
    {

        // ========================================== FIELDS =====================================================================
        private bool _isActive;

        private static readonly BehaviorSubject<bool> _debugStatus = new BehaviorSubject<bool>(false);
        

        // ========================================= PROPERTIES ==================================================================
        public bool IsActive { get => _isActive; }
        public static IObservable<bool> DebugStatus => _debugStatus; // allows behaviour to be subscribed to


        // ===================================== CONSTRUCTORS ====================================================================
        public CurrentModel(bool isActive)
        {
            _isActive = isActive;
        }

        // =====================================METHODS===========================================================================
        
        /// <summary>
        /// Toggles the current debug status between enabled and disabled.
        /// </summary>
        /// <remarks>This method switches the debug status to its opposite state. If the debug status is
        /// currently enabled,  it will be disabled, and vice versa. The change is propagated to all subscribers of the
        /// debug status.</remarks>
        public void ToggleDebugStatus()
        {
            bool prevStatus = _debugStatus.Value;
            _debugStatus.OnNext(!prevStatus);
        }

        /// <summary>
        /// Toggles the active state of the settings.
        /// </summary>
        /// <remarks>This method switches the current active state between active and inactive.  It is
        /// useful for enabling or disabling settings based on the current state.</remarks>
        public void ToggleActiveSettings()
        {
            _isActive = !_isActive;
        }
    }
}
