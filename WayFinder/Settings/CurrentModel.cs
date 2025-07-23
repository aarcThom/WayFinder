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
        private bool _isDebug = false;

        // ========================================= PROPERTIES ==================================================================
        public bool IsActive { get => _isActive; }
        public bool IsDebug { get => _isDebug; set => _isDebug = value; }


        // ===================================== CONSTRUCTORS ====================================================================
        public CurrentModel(bool isActive)
        {
            _isActive = isActive;
        }

        // =====================================METHODS===========================================================================
       
    }
}
