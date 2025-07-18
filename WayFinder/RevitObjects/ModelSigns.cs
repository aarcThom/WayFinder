using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.AppSettings;

namespace WayFinder.RevitObjects
{
    public class ModelSigns : IDisposable
    {
        // FIELDS==================================================================================

        private readonly IDisposable _subscription; // the subscription to the ModelSettings

        private string _modelName; // the model this class is bound to

        private bool _activeState; // whether WayFinder is currently active for the bound model

        private List<WallSign> _wallSigns = new List<WallSign>();


        // PROPERTIES===============================================================================
        public string ModelName { get => _modelName; }


        // CONSTRUCTOR==============================================================================
        public ModelSigns(string modelName)
        {
            _modelName = modelName;

            // subscribe to the model state
            _subscription = ModelSettings.Instance.FocusedDebugState.Subscribe(isActive =>
            {
                _activeState = ModelSettings.Instance.GetModelDebugState();


                // update the wall signs' state
                if (_wallSigns.Any())
                {
                    foreach (var wallSign in _wallSigns)
                    {
                        wallSign.SetActiveState(_activeState);
                    }
                }
                
            });
        }

        // METHODS==================================================================================
        public void AddWallSign()
        {
            _wallSigns.Add(new WallSign(_activeState));
        }

        public void Dispose()
        {
            _subscription?.Dispose(); // unsubscribe to the ModelSettings
        }
    }
}
