using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.Settings;

namespace WayFinder.RevitObjects
{
    public class ModelSigns
    {
        // FIELDS==================================================================================

        private string _modelName; // the model this class is bound to

        private bool _activeState; // whether WayFinder is currently active for the bound model

        private List<WallSign> _wallSigns = new List<WallSign>();


        // PROPERTIES===============================================================================
        public string ModelName { get => _modelName; }


        // CONSTRUCTOR==============================================================================
        public ModelSigns(string modelName)
        {
            
        }

        // METHODS==================================================================================
        public void AddWallSign()
        {
            _wallSigns.Add(new WallSign(_activeState));
        }
    }
}
