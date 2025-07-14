using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayFinder.AppSettings;

namespace WayFinder.RevitObjects
{
    public class WallSign : IDisposable
    {

        private readonly IDisposable _subscription;

        private string _modelName;

        public WallSign(string modelName)
        {
            _modelName = modelName;
            _subscription = ModelSettings.Instance.FocusedDebugState.Subscribe(isActive =>
            {
                bool state = ModelSettings.Instance.GetModelDebugState(_modelName);
                TaskDialog.Show("cool", $"The active state is {state}");
            });
        }


        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
